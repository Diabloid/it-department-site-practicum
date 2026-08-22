namespace it_department_site.Startup
{
   public static class StartupFactory
   {
      public static IStartup GetStartup(string[] args)
      {
         if(args.Contains("--migrate"))
         {
            return new MigrationStartup(args); 
         }
         
         if (args.Contains("--demo-data"))
         {
            return new DemoDataSeedStartup(args);
         }

         return new SiteStartup(args);
      }
   }
}
