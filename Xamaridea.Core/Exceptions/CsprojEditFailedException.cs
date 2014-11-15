using System;

namespace Xamaridea.Core.Exceptions
{
    public class CsprojEditFailedException : Exception
    {
        public string Csproj { get; set; }

        public CsprojEditFailedException(string csproj, Exception actualException) 
            : base("Unable to modify " + csproj, actualException)
        {
            Csproj = csproj;
        }
    }
}
