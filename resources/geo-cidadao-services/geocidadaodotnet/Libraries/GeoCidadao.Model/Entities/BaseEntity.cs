namespace GeoCidadao.Model.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.Now.ToUniversalTime();
        public DateTime? UpdatedAt { get; set; }
        // public DateTime? DeletedAt { get; set; }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}