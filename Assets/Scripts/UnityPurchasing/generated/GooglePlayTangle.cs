// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("CDIgP+56OkwAYjoktH94k7yrgLuxZm1qAdqZVXBs9ecgO0+K1PH98Z0k/Xwb6LPu3ix/luKzo393OoU7jjL0y6om6jZEVy7HeErOZom8ei1Q/LXD59bGgYDfxXReOhvbubXK6hKRn5CgEpGakhKRkZAij+AsJxLx5TJmgVIxhPSMYANS4NY7qLVHT2OgEpGyoJ2WmboW2BZnnZGRkZWQk+YlXQxY2YNkBo3Xgy/3J9OSv2U/a0oEZDDKnTBHJIhKx7lxDjXppYmVwkO/CJkzcc6mS9ZnXe8mo0tYELvdRS9uA4ffoA7A/sTOTNMnTE/p7rEV9mfOOEOCjh3RINHBhbU0OUVweADjB+WfexX0RGmLZVQP7TY/xmpIBVp3TD9yL5KTkZCR");
        private static int[] order = new int[] { 9,10,4,11,13,10,6,9,12,9,10,11,12,13,14 };
        private static int key = 144;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
