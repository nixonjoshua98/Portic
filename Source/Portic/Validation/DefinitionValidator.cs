using Portic.Exceptions;
using Portic.Messages;

namespace Portic.Validation
{
    internal static class DefinitionValidator
    {
        public static void ValidateMessageDefinitions(IEnumerable<IMessageDefinition> messageDefinitions)
        {
            bool isAllNamesUnique = messageDefinitions.Count() == messageDefinitions.DistinctBy(x => x.Name).Count();

            if (!isAllNamesUnique)
            {
                throw new DuplicateMessageNameException();
            }
        }
    }
}
