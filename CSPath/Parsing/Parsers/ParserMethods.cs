using System;
using System.Collections.Generic;
using System.Linq;

namespace CSPath.Parsing.Parsers
{
    public static class ParserMethods
    {
        public static IParser<TInput, TOutput> Any<TInput, TOutput>(Func<TInput, TOutput> produce)
        {
            return new AnyParser<TInput, TOutput>(produce);
        }

        public static IParser<TInput, TOutput> First<TInput, TOutput>(params IParser<TInput, TOutput>[] parsers)
        {
            return new FirstParser<TInput, TOutput>(parsers.ToArray());
        }

        public static IParser<TInput, TOutput> If<TInput, TOutput>(Func<ISequence<TInput>, bool> predicate, IParser<TInput, TOutput> parser)
        {
            return new IfParser<TInput, TOutput>(predicate, parser);
        }

        public static IParser<T, T> If<T>(Func<T, bool> predicate)
        {
            return new PredicateParser<T, T>(predicate, t => t);
        }

        public static IParser<TInput, TOutput> If<TInput, TOutput>(Func<TInput, bool> predicate, Func<TInput, TOutput> produce)
        {
            return new PredicateParser<TInput, TOutput>(predicate, produce);
        }

        public static IParser<TInput, TOutput> List<TInput, TItem, TOutput>(IParser<TInput, TItem> parser, Func<IReadOnlyList<TItem>, TOutput> produce, bool atLeastOne = false)
        {
            return new ListParser<TInput, TItem, TOutput>(parser, produce, atLeastOne);
        }

        public static IParser<TInput, TOutput> Rule<TInput, T1, T2, TOutput>(IParser<TInput, T1> p1, IParser<TInput, T2> p2, Func<T1, T2, TOutput> produce)
        {
            return new RuleParser<TInput, TOutput>(
                new IParser<TInput>[] { p1, p2 },
                (list) => produce((T1) list[0], (T2) list[1]));
        }

        public static IParser<TInput, TOutput> Rule<TInput, T1, T2, T3, TOutput>(IParser<TInput, T1> p1, IParser<TInput, T2> p2, IParser<TInput, T3> p3, Func<T1, T2, T3, TOutput> produce)
        {
            return new RuleParser<TInput, TOutput>(
                new IParser<TInput>[] { p1, p2, p3 },
                (list) => produce((T1) list[0], (T2) list[1], (T3) list[2]));
        }

        public static IParser<TInput, TOutput> Rule<TInput, T1, T2, T3, T4, TOutput>(IParser<TInput, T1> p1, IParser<TInput, T2> p2, IParser<TInput, T3> p3, IParser<TInput, T4> p4, Func<T1, T2, T3, T4, TOutput> produce)
        {
            return new RuleParser<TInput, TOutput>(
                new IParser<TInput>[] { p1, p2, p3, p4 },
                (list) => produce((T1) list[0], (T2) list[1], (T3) list[2], (T4) list[3]));
        }

        public static IParser<TInput, TOutput> Rule<TInput, T1, T2, T3, T4, T5, TOutput>(IParser<TInput, T1> p1, IParser<TInput, T2> p2, IParser<TInput, T3> p3, IParser<TInput, T4> p4, IParser<TInput, T5> p5, Func<T1, T2, T3, T4, T5, TOutput> produce)
        {
            return new RuleParser<TInput, TOutput>(
                new IParser<TInput>[] { p1, p2, p3, p4, p5 },
                (list) => produce((T1) list[0], (T2) list[1], (T3) list[2], (T4) list[3], (T5) list[4]));
        }

        public static IParser<TInput, TOutput> Rule<TInput, T1, T2, T3, T4, T5, T6, TOutput>(IParser<TInput, T1> p1, IParser<TInput, T2> p2, IParser<TInput, T3> p3, IParser<TInput, T4> p4, IParser<TInput, T5> p5, IParser<TInput, T6> p6, Func<T1, T2, T3, T4, T5, T6, TOutput> produce)
        {
            return new RuleParser<TInput, TOutput>(
                new IParser<TInput>[] { p1, p2, p3, p4, p5, p6 },
                (list) => produce((T1) list[0], (T2) list[1], (T3) list[2], (T4) list[3], (T5) list[4], (T6) list[5]));
        }

        public static IParser<TInput, TOutput> Rule<TInput, T1, T2, T3, T4, T5, T6, T7, TOutput>(IParser<TInput, T1> p1, IParser<TInput, T2> p2, IParser<TInput, T3> p3, IParser<TInput, T4> p4, IParser<TInput, T5> p5, IParser<TInput, T6> p6, IParser<TInput, T7> p7, Func<T1, T2, T3, T4, T5, T6, T7, TOutput> produce)
        {
            return new RuleParser<TInput, TOutput>(
                new IParser<TInput>[] { p1, p2, p3, p4, p5, p6, p7 },
                (list) => produce((T1) list[0], (T2) list[1], (T3) list[2], (T4) list[3], (T5) list[4], (T6) list[5], (T7) list[6]));
        }

        public static IParser<TInput, TOutput> Rule<TInput, T1, T2, T3, T4, T5, T6, T7, T8, TOutput>(IParser<TInput, T1> p1, IParser<TInput, T2> p2, IParser<TInput, T3> p3, IParser<TInput, T4> p4, IParser<TInput, T5> p5, IParser<TInput, T6> p6, IParser<TInput, T7> p7, IParser<TInput, T8> p8, Func<T1, T2, T3, T4, T5, T6, T7, T8, TOutput> produce)
        {
            return new RuleParser<TInput, TOutput>(
                new IParser<TInput>[] { p1, p2, p3, p4, p5, p6, p7, p8 },
                (list) => produce((T1) list[0], (T2) list[1], (T3) list[2], (T4) list[3], (T5) list[4], (T6) list[5], (T7) list[6], (T8) list[7]));
        }

        public static IParser<TInput, TOutput> Rule<TInput, T1, T2, T3, T4, T5, T6, T7, T8, T9, TOutput>(IParser<TInput, T1> p1, IParser<TInput, T2> p2, IParser<TInput, T3> p3, IParser<TInput, T4> p4, IParser<TInput, T5> p5, IParser<TInput, T6> p6, IParser<TInput, T7> p7, IParser<TInput, T8> p8, IParser<TInput, T9> p9, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TOutput> produce)
        {
            return new RuleParser<TInput, TOutput>(
                new IParser<TInput>[] { p1, p2, p3, p4, p5, p6, p7, p8, p9 },
                (list) => produce((T1) list[0], (T2) list[1], (T3) list[2], (T4) list[3], (T5) list[4], (T6) list[5], (T7) list[6], (T8) list[7], (T9) list[8]));
        }

        public static IParser<TInput, TOutput> SeparatedList<TInput, TItem, TSeparator, TOutput>(IParser<TInput, TItem> itemParser, IParser<TInput, TSeparator> sepParser, Func<IReadOnlyList<TItem>, TOutput> produce, bool atLeastOne = false)
        {
            return new SeparatedListParser<TInput, TItem, TSeparator, TOutput>(itemParser, sepParser, produce, atLeastOne);
        }

        public static IParser<TInput, TTransform> Transform<TInput, TOutput, TTransform>(IParser<TInput, TOutput> parser, Func<TOutput, TTransform> produce)
        {
            return new TransformParser<TInput, TOutput, TTransform>(parser, produce);
        }
    }
}