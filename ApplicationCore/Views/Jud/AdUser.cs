using ApplicationCore.Consts;

namespace ApplicationCore.Views.Jud;

public class AdUserViewModel
{
   public string crtid { get; set; } = String.Empty;
   public string sys { get; set; } = String.Empty;
   public string usrid { get; set; } = String.Empty;
   public string usrnm { get; set; } = String.Empty;
   public string dpt { get; set; } = String.Empty;
   public string nm { get; set; } = String.Empty;
   public string adusegrp { get; set; } = String.Empty;
}

public static class AdUserViewHelpers
{
   public static AppRoles ResolveRole(this AdUserViewModel model)
   {
      var val = model.adusegrp.ToUpper();
      if (val == "F") return AppRoles.Files;

      if (val == "CH3") return AppRoles.Clerk;
      if (val == "CV3") return AppRoles.Clerk;

      if (val == "CH4") return AppRoles.Recorder;
      if (val == "CV4") return AppRoles.Recorder;
      if (val == "DM") return AppRoles.Recorder;

      if (val == "SYSH") return AppRoles.IT;
      if (val == "SYSV") return AppRoles.IT;

      return AppRoles.UnKnown;
   }
}

