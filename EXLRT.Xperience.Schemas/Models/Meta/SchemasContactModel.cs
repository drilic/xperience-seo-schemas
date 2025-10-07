namespace EXLRT.Xperience.Schemas.Models.Meta
{
    public class SchemasContactModel
    {
        /// <summary>
        /// For example, a sales contact point, a PR contact point and so on.This property is used to specify the kind of contact point.
        /// </summary>
        public string ContactType { get; set; }

        public IEnumerable<string> AreaServed { get; set; }

        public IEnumerable<string> AvailableLanguages { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
    }
}
