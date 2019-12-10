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
                (OpCode(4), _ => ProcessCommand(
                        input, 
                        pInput,
                        start + 2,
                        (pOutput ?? Enumerable.Empty<int>()).Concat(new[] { input[input[start+1]]})
                    )),
                (x => x > 999, x => ProcessPrameterisedCommand(input, start, pInput, pOutput))
            );

        private static (int First, IEnumerable<int> Output) ProcessPrameterisedCommand(int[] input, int start = 0, int pInput = 0, IEnumerable<int> pOutput = null) =>
            input[start].ToString()
            .Map(x => (
                OpCode: int.Parse(x.Last().ToString()),
                Param1Type: int.Parse(x[x.Length - 3].ToString()),
                Param2Type: int.Parse(x[x.Length - 4].ToString()),
                Param3Type: x.Length == 5 ? int.Parse(x[0].ToString()) : -1
            ))
            .Map(x => (
                Op: OpLookup[x.OpCode],
                param1: x.Param1Type == 0 ? input[input[start+1]] : input[start+1],
                param2: x.Param2Type == 0 ? input[input[start+2]] : input[start+2],
                param3: x.Param3Type == -1 ? -1 : x.Param1Type == 0 ? input[input[start + 3]] : input[start + 3],
                outputLoc: input[start + 5]
            ))
            .Map(x => 
                ProcessCommand(
                    input.UpdateList(x.Op(x.param1, x.param2).Map(y => x.param3 == -1 ? y : x.Op(y, x.param3)), x.outputLoc),
                    pInput,
                    x.param3 == -1 ? start + 4 : start + 5,
                    pOutput
                ));


        public static (int First, IEnumerable<int> Output) Process(IEnumerable<int> input, int pInput = 0) =>
            ProcessCommand(input.ToArray(), pInput);

    }

}
