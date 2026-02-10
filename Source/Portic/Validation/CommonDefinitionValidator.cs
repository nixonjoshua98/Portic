using Portic.Exceptions;
using Portic.Messages;

namespace Portic.Validation
{
    internal static class CommonDefinitionValidator
    {
        public static void ValidateMessageDefinitions(IEnumerable<IMessageDefinition> messageDefinitions)
        {
            HashSet<string> seenMessageNames = [];

            foreach (var messageDefinition in messageDefinitions)
            {
                if (!seenMessageNames.Add(messageDefinition.Name))
                {
                    throw new DuplicateMessageNameException(messageDefinition.Name);
                }
            }
        }
    }
}
