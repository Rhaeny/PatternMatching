using System.Xml.Linq;

namespace AutomataLibrary
{
    public interface ISavable
    {
        XNode Save();
        void Load(XNode node);
    }
}
