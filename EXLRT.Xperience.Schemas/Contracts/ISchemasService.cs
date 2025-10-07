using EXLRT.Xperience.Schemas.Models.Meta;

namespace EXLRT.Xperience.Schemas.Contracts
{
    public interface ISchemasService
    {
        Task<SchemasConfigurationModel> GetConfigAsync();

        Task<SchemasLogoModel> GetLogoAsync();

        Task<IEnumerable<SchemasContactModel>> GetContactsAsync();
    }
}
