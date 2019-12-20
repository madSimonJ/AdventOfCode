using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using AdventOfCode.Common;

namespace AdventOfCode.Year2019.Common
{
    public static class OpcodeCalculator
    {
        public static int[] UpdateList(this int[] @this, int paramA, int paramB, Func<int, int, int> op, int outputPos) =>
                    @this.Select((y, i) =>
                        i == outputPos
                            ? op(@this[paramA], @this[paramB])
                            : y).ToArray();

        public static int[] UpdateList(this int[] @this, int item, int outputPos) =>
                    @this.Select((y, i) =>
                        i == outputPos
                            ? item
                            : y).ToArray();


        private static IDictionary<int, Func<int, int, int>> OpLookup = new Dictionary<int, Func<int, int, int>>
        {
            {1, (x, y) => x + y },
            {2, (x, y) => x * y }
        };

        private static Func<int, bool> OpCode(int code) =>
            x => x == code;

        private static Func<int, bool> OpCode(params int[] codes) =>
            x => codes.Contains(x);

        private static (int First, IEnumerable<int> Output) ProcessCommand(int[] input, int pInput = 0, int start = 0, IEnumerable<int> pOutput = null) =>
            input[start].Match(
                (OpCode(99), _ => (input[0], pOutput)),
                (OpCode(1, 2), _ => ProcessCommand(
                                        input.UpdateList(   input[start + 1],
                                                            input[start+2],
                                                            OpLookup[input[start]],
                                                            input[start + 3]), pInput, start + 4, pOutput)
                    ),
                (OpCode(3), _ => ProcessCommand(
                                    input.UpdateList(pInput, input[start + 1]),
                                    pInput,
                                    start + 2,
                                    pOutput
                                )),
                (OpCode(4, 104), _ => ProcessCommand(
                        input, 
                        pInput,
                        start + 2,
                        (pOutput ?? Enumerable.Empty<int>()).Concat(new[] { input[input[start+1]]})
                    )),
                (x => x > 99, _ => ProcessPrameterisedCommand(input, start, pInput, pOutput))
            );

        private static (int First, IEnumerable<int> Output) ProcessPrameterisedCommand(int[] input, int start = 0, int pInput = 0, IEnumerable<int> pOutput = null) =>
            input[start].ToString()
            .Map(x => (
                OpCode: int.Parse(x.Last().ToString()),
                Params: x.Take(x.Length - 2).Select(y => int.Parse(y.ToString())).Reverse()
            ))
            .Map(x => (
                Op: OpLookup[x.OpCode],
                Params: x.Params.Select((y, i) => 
                    y.Match(
                        (0, input[input[i + start + 1]]),
                        (1, input[i + start + 1])
                    )),
                OutputLoc: input[start + x.Params.Count() + 3]
            ))
            .Map(x => 
                ProcessCommand(
                    input.UpdateList(x.Params.ApplyOperations(x.Op), x.OutputLoc),
                    pInput,
                    start + x.Params.Count() + 2,
                    pOutput
                ));


        public static (int First, IEnumerable<int> Output) Process(IEnumerable<int> input, int pInput = 0) =>
            ProcessCommand(input.ToArray(), pInput);

    }

}
