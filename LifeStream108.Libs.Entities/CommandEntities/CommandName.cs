using System;
using System.Linq;

namespace LifeStream108.Libs.Entities.CommandEntities
{
    public class CommandName
    {
        public int Id { get; set; }

        public int SortOrder { get; set; }

        public int CommandId { get; set; }

        public string Alias { get; set; }

        public string SpacePositions { get; set; }

        public int LanguageId { get; set; }

        public string GetReadableAias()
        {
            try
            {
                if (string.IsNullOrEmpty(SpacePositions)) return Alias;

                string resultName = Alias;
                int[] spaceIndices = SpacePositions.Split(
                    new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(n => int.Parse(n)).ToArray();
                for (int i = spaceIndices.Length - 1; i >= 0; i--)
                {
                    resultName = resultName.Insert(spaceIndices[i] - 1, " ");
                }
                return resultName;
            }
            catch (Exception ex)
            {
                LoggerAccess.Logger.Error($"Error to build readable command name for {Alias}: {ex.Message}");
                return Alias;
            }
        }
    }
}
