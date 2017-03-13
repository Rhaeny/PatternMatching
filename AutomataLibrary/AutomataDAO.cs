using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AutomataLibrary
{
    public class AutomataDAO
    {
        public static void Save(AbstractFiniteAutomaton automaton, string fileName)
        {
            fileName = fileName + ".bin";
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, automaton);
            }
        }

        public static AbstractFiniteAutomaton Load(string fileName)
        {
            fileName = fileName + ".bin";
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                AbstractFiniteAutomaton automaton = bf.Deserialize(fs) as AbstractFiniteAutomaton;
                return automaton; 
            }
        }
    }
}
