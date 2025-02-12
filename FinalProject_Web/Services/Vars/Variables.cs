using Models.DataServiceModels;

namespace ServicesLibrary.Vars
{
    public class Variables
    {
        private static Lazy<Contacts> _contacts = new Lazy<Contacts>(() => new Contacts());
        private static Lazy<TitleModel> _tittleModelVars = new Lazy<TitleModel>(() => new TitleModel());
        static public Contacts Contacts { get; set; } = new Contacts();
        static public TitleModel TitleModelVars { get; set; } = new TitleModel();
    }
}
