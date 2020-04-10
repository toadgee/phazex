namespace PhazeX.Helpers
{
    using System.Collections;

    /// <summary>
    /// This class implements a ring list -- just like an array list except
    /// that no matter which index you throw at it, it will return a value
    /// (providing that the arraylist isn't empty).
    /// </summary>
    public class RingList : ArrayList
    {
        /// <summary>
        /// Returns the object at the specified index.
        /// </summary>
        public new object this[int index]
        {
            get
            {
                index %= this.Count;
                if (index < 0)
                {
                    index += this.Count;
                }

                return base[index];
            }
        }

        /// <summary>
        /// Get the next object based on which object we pass. Returns null if the object can't be found.
        /// </summary>
        /// <param name="o">The object to look for.</param>
        /// <returns>The next object</returns>
        public object NextObject(object o)
        {
            object retVal = null;
            for (int ctr = 0; ctr < this.Count; ctr++)
            {
                if (this[ctr] == o)
                {
                    // no need to check for bounds ... since this is a ring list!
                    retVal = this[ctr + 1];
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Gets the previous object based on which object we pass. Returns null if the object can't be found.
        /// </summary>
        /// <param name="o">The object to look for.</param>
        /// <returns>The previous object</returns>
        public object PrevObject(object o)
        {
            object retVal = null;
            for (int ctr = 0; ctr < this.Count; ctr++)
            {
                if (this[ctr] == o)
                {
                    // no need to check for bounds ... since this is a ring list!
                    retVal = this[ctr - 1];
                    break;
                }
            }

            return retVal;
        }
    }
}