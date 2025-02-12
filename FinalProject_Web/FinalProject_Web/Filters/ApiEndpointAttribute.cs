namespace FinalProject_Web.Filters
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    sealed class ApiEndpointAttribute : Attribute
    {
        public string Endpoint { get; set; }

        public ApiEndpointAttribute(string endpoint)
        {
            Endpoint = endpoint;
        }
    }
}
