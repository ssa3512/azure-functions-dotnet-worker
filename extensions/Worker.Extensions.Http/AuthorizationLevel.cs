namespace Microsoft.Azure.Functions.Worker.Extensions.Http
{
    public enum AuthorizationLevel
    {
        /// <summary>
        /// Allow access to anonymous requests.
        /// </summary>
        Anonymous = 0,

        /// <summary>
        /// Allow access to requests that include a valid user authentication token
        /// </summary>
        User = 1,

        /// <summary>
        /// Allow access to requests that include a function key
        /// </summary>
        Function = 2,

        /// <summary>
        /// Allows access to requests that include a system key
        /// </summary>
        System = 3,

        /// <summary>
        /// Allow access to requests that include the master key
        /// </summary>
        Admin = 4,
    }
}
