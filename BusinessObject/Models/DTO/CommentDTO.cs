namespace BusinessObject.Models.DTO
{
    public class CommentDTO
    {
        public long Commentid { get; set; }

        public int Userid { get; set; }

        public string? Content { get; set; }

        public DateTime? CreateAt { get; set; }

        public DateTime? UpdateAt { get; set; }

        public int? Serviceid { get; set; }

        public DateTime? GetUnspecifiedCreateAt()
        {
            return CreateAt?.Kind == DateTimeKind.Utc ? DateTime.SpecifyKind(CreateAt.Value, DateTimeKind.Unspecified) : CreateAt;
        }

        public DateTime? GetUnspecifiedUpdateAt()
        {
            return UpdateAt?.Kind == DateTimeKind.Utc ? DateTime.SpecifyKind(UpdateAt.Value, DateTimeKind.Unspecified) : UpdateAt;
        }

    }
}