using System;
using System.Collections;

namespace NSoft.NFramework.Nini.Util {
    public class OrderedListEnumerator : IDictionaryEnumerator {
        private int index = -1;
        private readonly ArrayList list;

        /// <summary>
        /// Instantiates an ordered list enumerator with an ArrayList.
        /// </summary>
        internal OrderedListEnumerator(ArrayList arrayList) {
            list = arrayList;
        }

        object IEnumerator.Current {
            get {
                if(index < 0 || index >= list.Count)
                    throw new InvalidOperationException();

                return list[index];
            }
        }

        public DictionaryEntry Current {
            get {
                if(index < 0 || index >= list.Count)
                    throw new InvalidOperationException();

                return (DictionaryEntry)list[index];
            }
        }

        public DictionaryEntry Entry {
            get { return (DictionaryEntry)Current; }
        }

        public object Key {
            get { return Entry.Key; }
        }

        public object Value {
            get { return Entry.Value; }
        }

        public bool MoveNext() {
            index++;
            if(index >= list.Count)
                return false;

            return true;
        }

        public void Reset() {
            index = -1;
        }
    }
}