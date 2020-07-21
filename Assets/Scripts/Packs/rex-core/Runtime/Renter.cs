using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace REXTools.REXCore
{
    public class Renter
    {
        protected List<Action> history;

        private static void modify<T>(T prop, T value)
        {
            prop = value;
        }

        public void rent<T>(T prop, Action<T> value)
        {
            T original = prop;

            history.Add(() => value(original));
        }

        public void endRent()
        {
            history[history.Count - 1]();
            history.RemoveAt(history.Count - 1);
        }

        #region "RentUse"
        public static void rentUse<T>(T prop, Action<T> value, Action use)
        {
            T originalValue = prop;

            use();

            value(originalValue);
        }

        public static void rentUse<
            T1,
            T2
            >(
            T1 prop1, Action<T1> value1,
            T2 prop2, Action<T2> value2,
            Action use)
        {
            T1 originalValue1 = prop1;
            //modify( prop1, value1);
            T2 originalValue2 = prop2;
            //modify( prop2, value2);

            use();

            value1(originalValue1);
            value2(originalValue2);
        }

        public static void rentUse<
            T1,
            T2,
            T3
            >(
             T1 prop1, Action<T1> value1,
             T2 prop2, Action<T2> value2,
             T3 prop3, Action<T3> value3,
             Action use)
        {
            T1 originalValue1 = prop1;
            //modify( prop1, value1);
            T2 originalValue2 = prop2;
            //modify( prop2, value2);
            T3 originalValue3 = prop3;
            //modify( prop3, value3);

            use();

            value1(originalValue1);
            value2(originalValue2);
            value3(originalValue3);
        }

        public static void rentUse<
            T1,
            T2,
            T3,
            T4
            >(
             T1 prop1, Action<T1> value1,
             T2 prop2, Action<T2> value2,
             T3 prop3, Action<T3> value3,
             T4 prop4, Action<T4> value4,
             Action use)
        {
            T1 originalValue1 = prop1;
            //modify( prop1, value1);
            T2 originalValue2 = prop2;
            //modify( prop2, value2);
            T3 originalValue3 = prop3;
            //modify( prop3, value3);
            T4 originalValue4 = prop4;
            //modify( prop4, value4);

            use();

            value1(originalValue1);
            value2(originalValue2);
            value3(originalValue3);
            value4(originalValue4);
        }
        #endregion
    }
}