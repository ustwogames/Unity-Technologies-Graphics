using System;
using UnityEditor.ShaderGraph.GraphDelta;

namespace UnityEditor.ShaderGraph.Defs
{
    internal class ScreenNode : IStandardNode
    {
        public static string Name => "Screen";
        public static int Version => 1;
        public static FunctionDescriptor FunctionDescriptor => new(
            Name,
@"Width = ScreenParams.x;
Height = ScreenParams.y;",
            new ParameterDescriptor[]
            {
                new ParameterDescriptor("Width", TYPE.Float, GraphType.Usage.Out),
                new ParameterDescriptor("Height", TYPE.Float, GraphType.Usage.Out),
                new ParameterDescriptor("ScreenParams", TYPE.Vec2, GraphType.Usage.Static, REF.ScreenParams)
            }
        );

        public static NodeUIDescriptor NodeUIDescriptor => new(
            Version,
            Name,
            tooltip: "Provides access to the screen's width and height parameters.",
            category: "Input/Scene",
            hasPreview: false,
            synonyms: Array.Empty<string>(),
            description: "pkg://Documentation~/previews/Screen.md",
            parameters: new ParameterUIDescriptor[2] {
                new ParameterUIDescriptor(
                    name: "Width",
                    tooltip: "Screen's width in pixels."
                ),
                new ParameterUIDescriptor(
                    name: "Height",
                    tooltip: "Screen's height in pixels."
                )
            }
        );
    }
}
