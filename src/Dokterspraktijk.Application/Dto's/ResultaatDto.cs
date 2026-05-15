namespace Dokterspraktijk.Application.Dto_s
{
    public class ResultaatDto
    {
        public bool IsGelukt { get; set; }
        public string Melding { get; set; }

        public ResultaatDto()
        {
            Melding = string.Empty;
        }

        public static ResultaatDto Succes(string melding)
        {
            return new ResultaatDto
            {
                IsGelukt = true,
                Melding = melding
            };
        }

        public static ResultaatDto Mislukt(string melding)
        {
            return new ResultaatDto
            {
                IsGelukt = false,
                Melding = melding
            };
        }
    }
}