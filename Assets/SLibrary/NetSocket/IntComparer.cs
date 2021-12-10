using System.Collections.Generic;

namespace SLibrary.NetSocket {
    public class IntComparer: IEqualityComparer<int> {
        public bool Equals(int x, int y) {
            return x == y;
        }

        public int GetHashCode(int obj) {
            return obj;
        }
    }
}