using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AutomataLibrary
{
    /// <summary>
    /// Contains static mathods allowing saving and loading of automata
    /// </summary>
    public class AutomataDAO
    {
        /// <summary>
        /// Saves automaton by using binary serialization to specified path.
        /// </summary>
        /// <param name="automaton">Automaton to save.</param>
        /// <param name="filePath">Path of saved automaton.</param>
        public static void Save(AbstractFiniteAutomaton automaton, string filePath)
        {
            filePath = filePath + ".bin";
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, automaton);
            }
        }

        /// <summary>
        /// Loads saved automaton from specified path by using binary deserialization.
        /// </summary>
        /// <param name="filePath">Path of saved automaton.</param>
        /// <returns>Loaded automaton.</returns>
        public static AbstractFiniteAutomaton Load(string filePath)
        {
            filePath = filePath + ".bin";
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                AbstractFiniteAutomaton automaton = bf.Deserialize(fs) as AbstractFiniteAutomaton;
                return automaton;
            }
        }
    }
}
