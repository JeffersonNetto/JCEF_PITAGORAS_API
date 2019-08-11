using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace WebApiCoreMongoDb.Services
{
    public class sPagSeguro
    {
        //static int ambiente = 1; //1 = Sandbox // 2 = Produção
        static string uriCheckoutProducao = "https://ws.pagseguro.uol.com.br/v2/checkout", urlPagamentoProducao = "https://pagseguro.uol.com.br/v2/checkout/payment.html?code=", uriConsultaProducao = "https://ws.pagseguro.uol.com.br/v3/transactions/";
        static string uriCheckoutSandbox = "https://ws.sandbox.pagseguro.uol.com.br/v2/checkout", urlPagamentoSandbox = "https://sandbox.pagseguro.uol.com.br/v2/checkout/payment.html?code=", uriConsultaSandbox = "https://ws.sandbox.pagseguro.uol.com.br/v3/transactions/";

        // Gerar pagamento do PagSeguro
        public static Dados GerarPagamento(Dados dados = null)
        {
            if (dados == null) return null;
            dados.stringConexao = "";
            try
            {
                //URI de checkout.
                string uri = dados.uriCheckout;
                //Conjunto de parâmetros/formData.
                System.Collections.Specialized.NameValueCollection postData =
                    new System.Collections.Specialized.NameValueCollection
                    {
                        {"email", dados.MeuEmail},
                        {"token", dados.MeuToken},
                        {"currency", "BRL"},
                        {"itemId1", dados.Referencia + "," + dados.Email},
                        {"itemDescription1", dados.TituloPagamento},
                        {"itemAmount1", dados.Valor.Replace(",",".")},
                        {"itemQuantity1", "1"},
                        {"itemWeight1", "000"},
                        {"reference", dados.Referencia},
                        {"senderName", dados.Nome},
                        {"senderAreaCode", dados.DDD},
                        {"senderPhone", dados.NumeroTelefone},
                        {"senderEmail", dados.ambiente == 1 ? "c09627948019150314322@sandbox.pagseguro.com.br" : dados.Email},
                        {"shippingAddressRequired", "false"},
                        {"redirectURL", "http://jcefpitagoras.com.br" },
                    };

                //String que receberá o XML de retorno.
                string xmlString = null;
                //Webclient faz o post para o servidor de pagseguro.
                using (WebClient wc = new WebClient())
                {
                    //Informa header sobre URL.
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    //Faz o POST e retorna o XML contendo resposta do servidor do pagseguro.
                    var result = wc.UploadValues(uri, postData);
                    //Obtém string do XML.
                    xmlString = Encoding.ASCII.GetString(result);
                }
                //Cria documento XML.
                XmlDocument xmlDoc = new XmlDocument();
                //Carrega documento XML por string.
                xmlDoc.LoadXml(xmlString);
                //Obtém código de transação (Checkout).
                var code = xmlDoc.GetElementsByTagName("code")[0];
                //Monta a URL para pagamento.                
                if (!code.InnerText.Equals(""))
                {
                    dados.CodigoAcesso = code.InnerText;
                    dados.stringConexao = string.Concat(dados.urlPagamento, code.InnerText);
                }
            }
            catch (System.Exception ex)
            {
                dados.CodigoAcesso = "";
                dados.stringConexao = "";
            }
            // Retorna com a URL para carregar na tela
            return dados;
        }

        // Validar situação do pagamento
        public static Dados ValidarPagamento(Dados dados = null)
        {
            if (dados == null) return null;
            Dados retorno = new Dados();
            try
            {
                //uri de consulta da transação.
                string uri = (dados.uriConsulta) + dados.CodigoAcesso +
                             "?email=" + dados.MeuEmail + "&token=" + dados.MeuToken;
                //Classe que irá fazer a requisição GET.
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
                //Método do webrequest.
                request.Method = "GET";
                //String que vai armazenar o xml de retorno.
                string xmlString = null;
                //Obtém resposta do servidor.
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    //Cria stream para obter retorno.
                    using (Stream dataStream = response.GetResponseStream())

                    {
                        //Lê stream.
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            //Xml convertido para string.
                            xmlString = reader.ReadToEnd();
                            //Cria xml document para facilitar acesso ao xml.
                            XmlDocument xmlDoc = new XmlDocument();
                            //Carrega xml document através da string com XML.
                            xmlDoc.LoadXml(xmlString);
                            //Busca elemento status do XML.
                            retorno.Status = xmlDoc.GetElementsByTagName("status")[0].InnerText;
                            //Fecha reader.
                            reader.Close();
                            //Fecha stream.
                            dataStream.Close();
                            //Verifica status de retorno.
                            //3 = Pago. Outas Tags verificar na documentação no site do PagSeguro                            
                            retorno.Email = xmlDoc.GetElementsByTagName("email")[0].InnerText;
                            retorno.Referencia = xmlDoc.GetElementsByTagName("reference")[0].InnerText;
                            retorno.CodigoAcesso = xmlDoc.GetElementsByTagName("code")[0].InnerText;
                        }
                    }
                    return retorno;
                }
            }
            catch
            {
                return null;
            }
        }
    }

    public class Dados
    {
        //token renata 
        public int ambiente = 1; //1 = Sandbox // 2 = Produção
        // Seus dados de acesso ao PagSeguro
        public string MeuEmail { get; set; }
        public string MeuToken { get; set; }
        // Dados de Envio para o PagSeguro
        public string Nome { get; set; }
        public string Email { get; set; }
        public string DDD { get; set; }
        public string NumeroTelefone { get; set; }
        public string Valor { get; set; }
        public string CodigoAcesso { get; set; }
        public string Referencia { get; set; }
        public string TituloPagamento { get; set; }
        // Dados de Retorno do PagSeguro
        public string Status { get; set; }
        public string stringConexao { get; set; }
        public string uriCheckout { get; set; }
        public string urlPagamento { get; set; }
        public string uriConsulta { get; set; }
        public Dados()
        {
            MeuEmail = ambiente == 1 ? "jeffersonneto_9@hotmail.com" : "rafaelsrodrigues1984@gmail.com";
            MeuToken = ambiente == 1 ? "11EDBFDB9DC1468E9D77FB150ECB8F35" : "8f5fcce7-bbb7-48cb-919d-9e673531af1fd22650bf44be87cf38655fc46babbc354618-43d5-47b0-b725-1f1a0923131f";
            
            TituloPagamento = "I Jornada Cientifica da Educacao Fisica";
            uriCheckout = ambiente == 1 ? "https://ws.sandbox.pagseguro.uol.com.br/v2/checkout" : "https://ws.pagseguro.uol.com.br/v2/checkout";
            urlPagamento = ambiente == 1 ? "https://sandbox.pagseguro.uol.com.br/v2/checkout/payment.html?code=" : "https://pagseguro.uol.com.br/v2/checkout/payment.html?code=";
            uriConsulta = ambiente == 1 ? "https://ws.sandbox.pagseguro.uol.com.br/v3/transactions/" : "https://ws.pagseguro.uol.com.br/v3/transactions/";
        }
    }

    public static class sValidar
    {
        // Validar conexão com a internet
        [DllImport("wininet.dll")]

        private static extern bool InternetGetConnectedState(out int description, int reservedValue);

        public static bool IsConnected()
        {
            int description;
            return InternetGetConnectedState(out description, 0);
        }

        // Validar e-mail

        public static bool ValidarEmail(string email)
        {
            return Regex.IsMatch(email, ("(?<user>[^@]+)@(?<host>.+)"));
        }

        // Valida texto como decimal

        public static bool ValidarDecimal(string valor = "")
        {
            bool retorno = true;

            try
            {
                var tmp = Convert.ToDecimal(valor);
            }
            catch
            {
                retorno = false;
            }
            return retorno;
        }
        
        // Retornar valor em formato monetáio
        public static string ValidarMoeda(string valor = "", bool casadecimal = true)
        {
            try
            {
                // Verifica se o valor passado é um valor numérico
                if (!ValidarDecimal(valor))
                    return casadecimal ? "0,00" : "0";
                // Se tiver mais de um sinal negativo, remove do valor
                if (valor.Contains("-") && valor.IndexOf("-") > 0)
                    valor = valor.Replace("-", "");
                // Se não for um valor válido, retorna 0
                if (valor.Equals("") || valor.Equals("-,") || valor.Equals(",-")) valor = casadecimal ? "0,00" : "0";
                if (!valor.Equals("") && Convert.ToDecimal(valor).Equals(0)) return casadecimal ? "0,00" : "0";
                // Retornar o valor validado
                return valor.Equals("")
                    ? casadecimal ? "0,00" : "0"
                    : Math.Round(Convert.ToDecimal(valor), 2).ToString("N" + (casadecimal ? "2" : "0"));
            }
            catch
            {
                return "0,00";
            }
        }
    }
}
