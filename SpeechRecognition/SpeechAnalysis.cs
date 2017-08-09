namespace SpeechRecognition
{
    public class SpeechAnalysis
    {
        public bool SpeechIsMatchPattern(string speech)
        {
            foreach (string pattern in Consts.patterns)
            {
                if (speech.Contains(pattern))
                {
                    return true;
                }
            }

            return false;
        }
    }
}