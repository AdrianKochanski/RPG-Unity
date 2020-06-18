
using System.Collections.Generic;

namespace RPG.Stats
{
    public interface IModifierProvider
    {
        IEnumerable<float> GetAdditiveModifiersFor(Stat stat);
        IEnumerable<float> GetPercentageModifiersFor(Stat stat);
    }
}