namespace Meetup.Bot.Modules.Diagnostics
{
    public class DiagnosticFormatting
    {
        public string MessageTemplate { get; }
        public object[] PropertyValues { get; }

        public DiagnosticFormatting(string messageTemplate, object[] propertyValues)
        {
            MessageTemplate = messageTemplate;
            PropertyValues = propertyValues;
        }
    }
}