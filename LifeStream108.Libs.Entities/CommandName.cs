using System;
using System.Linq;

namespace LifeStream108.Libs.Entities
{
    public class CommandName
    {
        public virtual int Id { get; set; }

        public virtual int SortOrder { get; set; }

        public virtual int CommandId { get; set; }

        public virtual string Alias { get; set; }

        public virtual string SpacePositions { get; set; }

        public virtual int LanguageId { get; set; }

        public virtual string GetReadableAias()
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
