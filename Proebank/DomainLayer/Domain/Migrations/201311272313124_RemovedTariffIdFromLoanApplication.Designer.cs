// <auto-generated />
namespace Infrastructure.Migrations
{
    using System.CodeDom.Compiler;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    using System.Resources;
    
    [GeneratedCode("EntityFramework.Migrations", "6.0.1-21010")]
    public sealed partial class RemovedTariffIdFromLoanApplication : IMigrationMetadata
    {
        private readonly ResourceManager Resources = new ResourceManager(typeof(RemovedTariffIdFromLoanApplication));
        
        string IMigrationMetadata.Id
        {
            get { return "201311272313124_RemovedTariffIdFromLoanApplication"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return Resources.GetString("Source"); }
        }
        
        string IMigrationMetadata.Target
        {
            get { return Resources.GetString("Target"); }
        }
    }
}