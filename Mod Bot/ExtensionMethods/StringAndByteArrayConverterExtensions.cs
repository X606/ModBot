namespace InternalModBot
{
    /// <summary>
    /// Adds the ToBytes method to <see langword="string"/> and the RawBytesToString method to <see langword="byte"/>[]
    /// </summary>
    public static class StringAndByteArrayConverterExtensions
    {
        /// <summary>
        /// Converts each <see langword="char"/> in this <see langword="string"/> to a <see langword="byte"/>, and puts them in a <see langword="bye"/>[] in order
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this string me)
        {
            byte[] bytes = new byte[me.Length];
            for (int i = 0; i < me.Length; i++)
            {
                byte byteValue = (byte)me[i];
                bytes[i] = byteValue;
            }

            return bytes;
        }

        /// <summary>
        /// Converts each <see langword="byte"/> in the <see langword="byte"/>[] to a <see langword="char"/>, then combines them to a <see langword="string"/> in order
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public static string RawBytesToString(this byte[] me)
        {
            char[] characters = new char[me.Length];
            for (int i = 0; i < me.Length; i++)
            {
                char character = (char)me[i];
                characters[i] = character;
            }

            return new string(characters);
        }
    }

}
