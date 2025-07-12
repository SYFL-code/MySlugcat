using System;
using UnityEngine;
using System.Linq;
using IL.Menu;
using Mono.Cecil.Cil;
using MonoMod.Cil;


namespace MySlugcat
{
    public class Intros
    {
        //static bool random = 1 != UnityEngine.Random.Range(1, 3);
        static bool random = false;

        public static readonly string[] titles = new string[]
        {
            "MySlugcat"
        };


        public static void Hook()
        {
            IntroRoll.ctor += IntroRoll_ctor;
        }

        public static void IntroRoll_ctor(ILContext il)
        {

/*            if (1 != UnityEngine.Random.Range(1, 3))
            {
                goto Skip;
            }*/

            ILCursor ilcursor = new ILCursor(il);
            ILCursor ilcursor2 = ilcursor;
            Func<Instruction, bool>[] array = new Func<Instruction, bool>[1];
            array[0] = ((Instruction i) => ILPatternMatchingExt.MatchLdstr(i, "Intro_Roll_C_"));
            bool flag;
            if (ilcursor2.TryGotoNext(array))
            {
                ILCursor ilcursor3 = ilcursor;
                MoveType moveType = (MoveType)2;
                Func<Instruction, bool>[] array2 = new Func<Instruction, bool>[1];
                array2[0] = ((Instruction i) => ILPatternMatchingExt.MatchCallOrCallvirt<string>(i, "Concat"));
                flag = ilcursor3.TryGotoNext(moveType, array2);
            }
            else
            {
                flag = false;
            }
            bool flag2 = flag;
            if (flag2)
            {
                ilcursor.Emit(OpCodes.Ldloc_3);
                ilcursor.EmitDelegate<Func<string, string[], string>>(delegate (string titleImage, string[] oldTitleImages)
                {
                    oldTitleImages = (random ? titles.Concat(oldTitleImages).ToArray<string>() : titles);
                    return GetVanillaIntros(oldTitleImages);
                });
            }
            ilcursor.Index = 0;
            ILCursor ilcursor4 = ilcursor;
            Func<Instruction, bool>[] array3 = new Func<Instruction, bool>[1];
            array3[0] = ((Instruction i) => ILPatternMatchingExt.MatchLdstr(i, "Intro_Roll_C_"));
            bool flag3;
            if (ilcursor4.TryGotoNext(array3))
            {
                ILCursor ilcursor5 = ilcursor;
                Func<Instruction, bool>[] array4 = new Func<Instruction, bool>[1];
                array4[0] = ((Instruction i) => ILPatternMatchingExt.MatchLdstr(i, "Intro_Roll_C"));
                flag3 = ilcursor5.TryGotoPrev(array4);
            }
            else
            {
                flag3 = false;
            }
            bool flag4 = flag3;
            if (flag4)
            {
                ilcursor.EmitDelegate<Func<string, string>>((string titleImage) => GetVanillaIntros(titles));
            }

        Skip:
            {

            }

        }

        private static string GetVanillaIntros(string[] titleImages)
        {
            string result = "Intro_Roll_C_" + titleImages[UnityEngine.Random.Range(0, titleImages.Length)];

            return result;
        }

    }
}

