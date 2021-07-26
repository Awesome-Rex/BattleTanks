using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REXTools
{
    public interface ICloneable<T>
    {
        public T Clone();
    }
}