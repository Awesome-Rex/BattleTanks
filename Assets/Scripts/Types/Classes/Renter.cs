using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Renter
{
    private static void modify<T> (ref T prop, T value)
    {
        prop = value;
    }

    public static void rent<T>(Action use, ref T prop, T value)
    {
        T originalValue = prop;
        modify(ref prop, value);

        use();

        modify(ref prop, originalValue);
    }

    public static void rent<
        T1, 
        T2
        >(Action use, 
        ref T1 prop1, T1 value1, 
        ref T2 prop2, T2 value2)
    {
        T1 originalValue1 = prop1;
        modify(ref prop1, value1);

        T2 originalValue2 = prop2;
        modify(ref prop2, value2);
        
        use();

        modify(ref prop1, originalValue1);

        modify(ref prop2, originalValue2);
    }

    public static void rent<
        T1, 
        T2, 
        T3
        >(Action use, 
        ref T1 prop1, T1 value1, 
        ref T2 prop2, T2 value2, 
        ref T3 prop3, T3 value3)
    {
        T1 originalValue1 = prop1;
        modify(ref prop1, value1);

        T2 originalValue2 = prop2;
        modify(ref prop2, value2);

        T3 originalValue3 = prop3;
        modify(ref prop3, value3);

        use();

        modify(ref prop1, originalValue1);

        modify(ref prop2, originalValue2);

        modify(ref prop3, originalValue3);
    }

    public static void rent<
        T1, 
        T2, 
        T3, 
        T4
        >(Action use,
        ref T1 prop1, T1 value1,
        ref T2 prop2, T2 value2,
        ref T3 prop3, T3 value3,
        ref T4 prop4, T4 value4)
    {
        T1 originalValue1 = prop1;
        modify(ref prop1, value1);

        T2 originalValue2 = prop2;
        modify(ref prop2, value2);

        T3 originalValue3 = prop3;
        modify(ref prop3, value3);

        T4 originalValue4 = prop4;
        modify(ref prop4, value4);

        use();

        modify(ref prop1, originalValue1);

        modify(ref prop2, originalValue2);

        modify(ref prop3, originalValue3);

        modify(ref prop4, originalValue4);
    }
}
