using Microsoft.AspNetCore.Http;

namespace DatingApp.API.Helpers {
    public static class Extensions {
        public static void AddApplicationError (this HttpResponse ressponse, string message) {
            ressponse.Headers.Add ("Application-Error", message);
            ressponse.Headers.Add ("Access-Control-Expose-Headers", "Application-Error");
            ressponse.Headers.Add ("Access-Control-Allow-Origin", "*"); //RM: we need it for core-2.2; for core-3.0 header added automaticaly.
        }
    }
}