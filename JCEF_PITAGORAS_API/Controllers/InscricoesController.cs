using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using WebApiCoreMongoDb.Models;
using WebApiCoreMongoDb.Services;

namespace WebApiCoreMongoDb.Controllers
{
    [Produces("application/json")]
    [Route("api/Inscricoes")]
    public class InscricoesController : Controller
    {
        private Context context;        

        public InscricoesController()
        {
            context = new Context();            
        }

        // GET: api/Inscricoes
        [HttpGet]
        public async Task<List<Inscricao>> Get()
        {
            return await context.Inscricoes.Find(_ => true).ToListAsync();
        }

        // GET: api/Inscricoes/1
        [HttpGet("{id}")]
        public async Task<Inscricao> Get(int id)
        {
            return await context.Inscricoes.Find(_ => _.Codigo == id).FirstOrDefaultAsync();
        }

        // GET: api/Inscricoes/Login        
        [HttpPost("login")]
        public async Task<Inscricao> Login([FromBody]Inscricao inscricao)
        {
            return await context.Inscricoes.Find(_ => _.Cpf == inscricao.Cpf && _.Email == inscricao.Email).FirstOrDefaultAsync();
        }

        // POST: api/Inscricoes
        [HttpPost]
        public IActionResult PostAsync([FromBody]Inscricao inscricao)
        {
            if (inscricao == null)
                return BadRequest();

            //inscricao.Categoria = await context.Categorias.Find(_ => _.Codigo == inscricao.Categoria.Codigo).FirstOrDefaultAsync();
            //inscricao.StatusPagamento = await context.StatusPagamento.Find(_ => _.Codigo == inscricao.StatusPagamento.Codigo).FirstOrDefaultAsync();

            context.Inscricoes.InsertOne(inscricao);

            if (string.IsNullOrWhiteSpace(inscricao.Id))
                return NotFound();

            inscricao.Eventos.ForEach(evento => {
                context.Eventos.UpdateOne(Builders<Evento>.Filter.Eq(e => e.Codigo, evento.Codigo), Builders<Evento>.Update.Set(e => e.qtdVagas, --evento.qtdVagas));
            });

            string urlPagSeguro = GerarPagamento(inscricao);

        //    _emailSender.SendEmailAsync(inscricao.Email, "Pré-inscrição realizada com sucesso", @"Seja bem vindo a II Semana de Estudos Farmacêuticos da Faculdade Pitágoras! É um prazer tê-lo conosco em nosso evento.<br /><br />
        //Sua pré-inscrição foi realizada com sucesso! Para confirmá-la, acesse o link abaixo e efetue o pagamento.<br /><br />
        //<a href='"+urlPagSeguro+"'>"+urlPagSeguro+@"</a><br /><br />
        //Após o pagamento, você será redirecionado ao site. Caso isso não aconteça automaticamente, clique no link 'voltar para a loja'.
        //<br /><br />
        //Atenciosamente,<br /><br />
        //II Semana de Estudos Farmacêuticos - Faculdade Pitágoras Ipatinga <br /><br />
        //Comissão Organizadora <br /><br /> ");

            return Ok(new { inscricao.Email, sucesso = true, mensagem = "Inscrição realizada com sucesso", urlPagSeguro }); //return Ok(new { inscricao.Email, sucesso = true, mensagem = "Inscrição realizada com sucesso", urlPagSeguro = GerarPagamento(inscricao) });
        }

        // PUT: api/Inscricoes/5
        [HttpPut("{id}")]
        public async Task PutAsync(int id, [FromBody]Inscricao inscricao)
        {
            var filter = Builders<Inscricao>.Filter.Eq(_ => _.Codigo, id);
            var update = Builders<Inscricao>.Update
                            .Set(_ => _.Nome, inscricao.Nome)
                            .Set(_ => _.Cpf, inscricao.Cpf)
                            .Set(_ => _.Telefone, inscricao.Telefone)
                            .Set(_ => _.Email, inscricao.Email)
                            .Set(_ => _.Categoria, inscricao.Categoria)
                            .Set(_ => _.StatusPagamento, inscricao.StatusPagamento);

            await context.Inscricoes.UpdateOneAsync(filter, update);
        }

        // DELETE: api/Inscricoes/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            //await context.Inscricoes.DeleteOneAsync(Builders<Inscricao>.Filter.Eq("Codigo", id));
            await context.Inscricoes.DeleteOneAsync(_ => _.Codigo == id);
        }

        private string GerarPagamento(Inscricao inscricao)
        {
            inscricao.Telefone = inscricao.Telefone.Replace("(", "").Replace(")", "").Replace(" ", "");

            // Verifica se os dados foram digitados, caso não tenha sido, gera uma excessão
            if (!sValidar.IsConnected())
                throw new Exception("É necessário estar conectado à internet para gerar o pagamento!");

            // Povoar atributos da entidade para envio
            Dados dadosEnvio = new Dados
            {
                Nome = inscricao.Nome,
                Email = inscricao.Email,
                DDD = inscricao.Telefone.Substring(0, 2),
                NumeroTelefone = inscricao.Telefone.Length == 11 ? inscricao.Telefone.Substring(2, 9) : inscricao.Telefone.Length == 10 ? inscricao.Telefone.Substring(2, 8) : null,
                Referencia = inscricao.Id.ToString(),
                Valor = inscricao.Categoria.Valor.ToString("n2")
            };

            // Enviar dados para o PagSeguro para gerar o pagamento
            dadosEnvio = sPagSeguro.GerarPagamento(dadosEnvio);

            // Se retornar uma string para abrir a página do pagseguro quer dizer que foi gerado o código de acesso
            if (!string.IsNullOrWhiteSpace(dadosEnvio.stringConexao))            
                context.Inscricoes.UpdateOne(Builders<Inscricao>.Filter.Eq(_ => _.Id, inscricao.Id), Builders<Inscricao>.Update.Set(_ => _.PaymentCode, dadosEnvio.CodigoAcesso));
            
            return dadosEnvio.stringConexao;
        }

        // GET: api/Inscricoes/retornopagamento/1
        [HttpGet("retornopagamento/{tid}")]
        public IActionResult RetornoPagamento(string tid)
        {
            if (!sValidar.IsConnected())
                throw new Exception("Sem conexão com a internet");

            string mensagem = null;
            bool sucesso = false;

            // Criar nova entidade com os dados para consulta
            Dados meusDados = new Dados
            {
                CodigoAcesso = tid,
            };

            // Consulta junto ao PagSeguro se o pagamento foi efetuado
            Dados retonoConsulta = sPagSeguro.ValidarPagamento(meusDados);

            Inscricao inscricao = new Inscricao();

            // Se o retorno não for nulo, quer dizer que o pagamento foi efetuado
            if (retonoConsulta != null)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        inscricao = context.Inscricoes.Find(_ => _.Id == retonoConsulta.Referencia).FirstOrDefault();

                        if (inscricao != null)
                        {
                            // Se o status for 3, foi aprovado, caso contrário continua aguardando o pagamento
                            if (retonoConsulta.Status == "3")
                            {
                                mensagem = "Pagamento realizado com sucesso";
                                sucesso = true;

                                if (inscricao.StatusPagamento.Codigo == 1)
                                {
                                    context.Inscricoes.UpdateOne(
                                        Builders<Inscricao>.Filter.Eq(_ => _.Id, inscricao.Id),
                                        Builders<Inscricao>.Update
                                        .Set(_ => _.TransactionId, inscricao.TransactionId = tid)
                                        .Set(_ => _.DataPagamento, inscricao.DataPagamento = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"))
                                        .Set(_ => _.StatusPagamento, inscricao.StatusPagamento = new StatusPagamento() { Codigo = 2, Descricao = "Pago" })
                                        );

          //                          string msg = "Olá <em>" + inscricao.Nome + "</em><br /><br />Sua inscrição está confirmada!<br /><br />Eventos escolhidos:<br /><br />";

          //                          foreach (Evento ev in inscricao.Eventos)
          //                          {
          //                              string palestrantes = "";

          //                              ev.Palestrantes.ForEach(p =>
          //                              {
          //                                  if (string.IsNullOrWhiteSpace(palestrantes))
          //                                      palestrantes += p.Nome;
          //                                  else
          //                                      palestrantes += ", " + p.Nome;
          //                              });

          //                              msg += "<strong>" + ev.Titulo + "</strong><br />" + ev.Descricao + "<br />" + ev.DataHora + "<br />" + palestrantes + "<br /><br />";
          //                          }

          //                          msg += @"<br/><br/>
          //Atenciosamente,<br/><br/>
          //II Semana de Estudos Farmacêuticos - Faculdade Pitágoras Ipatinga<br/><br/>
          //Comissão Organizadora<br /><br />";

          //                          _emailSender.SendEmailAsync(inscricao.Email, "Inscrição confirmada com sucesso", msg);
                                }
                            }
                            else
                            {
                                mensagem = "Pagamento não confirmado";

                                context.Inscricoes.UpdateOne(
                                        Builders<Inscricao>.Filter.Eq(_ => _.Id, inscricao.Id),
                                        Builders<Inscricao>.Update
                                        .Set(_ => _.TransactionId, inscricao.TransactionId = tid)
                                        .Set(_ => _.DataPagamento, inscricao.DataPagamento = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"))
                                        );
                            }

                            scope.Complete();
                        }                        
                    }
                    catch (Exception ex)
                    {
                        mensagem = ex.Message;                
                    }
                }
            }
            
            return Ok(new { inscricao, sucesso, mensagem });
        }
    }
}