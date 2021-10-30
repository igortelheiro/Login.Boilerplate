namespace Login.Domain
{
    public record EmailRequest
    {
        public string DestinationEmail { get; init; }

        public string Subject { get; init; }

        public string Content { get; init; }

        public string Template { get; init; }
    }
}
