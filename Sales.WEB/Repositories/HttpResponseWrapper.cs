namespace Sales.WEB.Repositories
{
    public class HttpResponseWrapper<T>
    {
        public bool Error { get; set; }
        public T? Response { get; set; }
        public HttpResponseMessage HttpResponseMessage { get; set; }

        public HttpResponseWrapper(T? response, bool error, HttpResponseMessage httpResponseMessage)
        {
            Error = error;
            Response = response;
            HttpResponseMessage = httpResponseMessage;
        }

        public async Task<string?> GetErrorMessageAsync()
        {
            if (!Error)
            {
                return null;
            }

            var statusCode = HttpResponseMessage.StatusCode;

            switch (statusCode)
            {
                case System.Net.HttpStatusCode.NotFound:
                    return "Recurso no encontrado";

                case System.Net.HttpStatusCode.BadRequest:
                    return await HttpResponseMessage.Content.ReadAsStringAsync();

                case System.Net.HttpStatusCode.Unauthorized:
                    return "Tiene que loguearse para realizar esta operacion";

                case System.Net.HttpStatusCode.Forbidden:
                    return "No tiene permisos para realizar esta operacion";

                default:
                    return "Ha ocurrido un error inesperado";
            }
        }
    }
}