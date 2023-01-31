using Usage = UnityEditor.ShaderGraph.GraphDelta.GraphType.Usage;

namespace UnityEditor.ShaderGraph.Defs
{
    internal class NoiseSineWaveNode : IStandardNode
    {
        public static string Name => "NoiseSineWave";
        public static int Version => 1;

        public static FunctionDescriptor FunctionDescriptor => new(
            Name,
@"    sinIn = sin(In);
    Out = sinIn + lerp(Min, Max, frac(sin((sinIn - sin(In + 1.0)) * (12.9898 + 78.233))*43758.5453));",
            new ParameterDescriptor[]
            {
                new ParameterDescriptor("In", TYPE.Vector, Usage.In),
                new ParameterDescriptor("Min", TYPE.Float, Usage.In, new float[] { -0.5f }),
                new ParameterDescriptor("Max", TYPE.Float, Usage.In, new float[] { 0.5f }),
                new ParameterDescriptor("Out", TYPE.Vector, Usage.Out),
                new ParameterDescriptor("sinIn", TYPE.Float, Usage.Local)
            }
        );

        public static NodeUIDescriptor NodeUIDescriptor => new(
            Version,
            Name,
            displayName: "Noise Sine Wave",
            tooltip: "Creates a sine wave with noise added to the amplitude for randomness.",
            category: "Math/Wave",
            synonyms: new string[3] { "wave", "noise", "sine" },
            description: "pkg://Documentation~/previews/NoiseSineWave.md",
            parameters: new ParameterUIDescriptor[4] {
                new ParameterUIDescriptor(
                    name: "In",
                    tooltip: "the input value"
                ),
                new ParameterUIDescriptor(
                    name: "Min",
                    tooltip: "Minimum value for noise intensity"
                ),
                new ParameterUIDescriptor(
                    name: "Max",
                    tooltip: "Maximum value for noise intensity"
                ),
                new ParameterUIDescriptor(
                    name: "Out",
                    tooltip: "a sine wave with noise added to the amplitude for randomness"
                )
            }
        );
    }
}
