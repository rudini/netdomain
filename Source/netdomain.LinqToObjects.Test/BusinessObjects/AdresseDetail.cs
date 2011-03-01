

namespace netdomain.LinqToObjects.Test.BusinessObjects
{
    public class AdresseDetail
    {
        public virtual int Id { get; set; }

        /// <summary>
        /// Gets or sets the person ID.
        /// </summary>
        /// <value>The person ID.</value>
        public virtual Adresse Adresse { get; set; }
    }
}