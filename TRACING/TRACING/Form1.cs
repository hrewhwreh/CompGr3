using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace TRACING
{
    public partial class Form1 : Form
    {
        int BasicProgramID;
        int BasicVertexShader;
        int BasicFragmentShader;

        void loadShader(String filename, ShaderType type, int program, out int address)
        {
            address = GL.CreateShader(type);
            using (System.IO.StreamReader sr = new System.IO.StreamReader(filename))
            {
                GL.ShaderSource(address, sr.ReadToEnd());
            }
            GL.CompileShader(address);
            GL.AttachShader(program, address);
            Console.WriteLine(GL.GetShaderInfoLog(address));
        }

        void InitShaders()
        {
            BasicProgramID = GL.CreateProgram();
            loadShader("..\\..\\Shaders\\raytracing.vert", ShaderType.VertexShader, BasicProgramID, out BasicVertexShader);
            loadShader("..\\..\\Shaders\\raytracing.frag", ShaderType.FragmentShader, BasicProgramID, out BasicFragmentShader);
            GL.LinkProgram(BasicProgramID);
            int status = 0;
            GL.GetProgram(BasicProgramID, GetProgramParameterName.LinkStatus, out status);
            Console.WriteLine(GL.GetProgramInfoLog(BasicProgramID));
        }

        private static bool Init()
        {
            GL.Enable(EnableCap.ColorMaterial);
            GL.ShadeModel(ShadingModel.Smooth);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            return true;
        }

        Material[] fillMaterials()
        {
            Material[] materials = new Material[8];
            Vector4 lightCoefs = new Vector4(0.4f, 0.9f, 0.0f, 512.0f);
            materials[0] = new Material(new Vector3(0, 1, 0), lightCoefs, 0.5f, 1, 1);
            materials[1] = new Material(new Vector3(0, 0, 1), lightCoefs, 0.5f, 1, 1);
            materials[2] = new Material(new Vector3(1, 0, 0), lightCoefs, 0.5f, 1, 1);
            materials[3] = new Material(new Vector3(1, 1, 1), lightCoefs, 0.5f, 1, 1);
            materials[4] = new Material(new Vector3(1, 1, 1), lightCoefs, 0.5f, 1, 1);
            materials[5] = new Material(new Vector3(1, 1, 1), lightCoefs, 0.5f, 1, 1);
            materials[6] = new Material(new Vector3(1, 1, 1), new Vector4(0.4f, 0.9f, 0.9f, 50.0f), 0.8f, 1.5f, 2);
            materials[7] = new Material(new Vector3(1, 0, 1), lightCoefs, 0.5f, 1, 1);
            return materials;
        }

        void initMaterials()
        {
            Material[] materials = fillMaterials();
            int location;
            for (int i = 0; i < materials.Length; i++)
            {
                location = GL.GetUniformLocation(BasicProgramID, "uMaterials[" + i + "].Color");
                GL.Uniform3(location, materials[i].Color);
                location = GL.GetUniformLocation(BasicProgramID, "uMaterials[" + i + "].LightCoeffs");
                GL.Uniform4(location, materials[i].LightCoeffs);
                location = GL.GetUniformLocation(BasicProgramID, "uMaterials[" + i + "].ReflectionCoef");
                GL.Uniform1(location, materials[i].ReflectionCoef);
                location = GL.GetUniformLocation(BasicProgramID, "uMaterials[" + i + "].RefractionCoef");
                GL.Uniform1(location, materials[i].RefractionCoef);
                location = GL.GetUniformLocation(BasicProgramID, "uMaterials[" + i + "].MaterialType");
                GL.Uniform1(location, materials[i].MaterialType);
            }

        }

        int vbo_position;
        int attribute_vpos = 1;

        void Draw()
        {
            GL.ClearColor(Color.AliceBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            ;
            GL.UseProgram(BasicProgramID);
            initMaterials();
            // Quad
            GL.Color3(Color.White);
            GL.Begin(PrimitiveType.Quads);

            GL.TexCoord2(0, 1);
            GL.Vertex2(-1, -1);

            GL.TexCoord2(1, 1);
            GL.Vertex2(1, -1);

            GL.TexCoord2(1, 0);
            GL.Vertex2(1, 1);

            GL.TexCoord2(0, 0);
            GL.Vertex2(-1, 1);

            GL.End();
            glControl1.SwapBuffers();
            GL.UseProgram(0);
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            InitShaders();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            Draw();
        }

        public struct Material
        {
            //diffuse color
            public Vector3 Color;
            // ambient, diffuse and specular coeffs
            public Vector4 LightCoeffs;
            // 0 - non-reflection, 1 - mirror
            public float ReflectionCoef;
            public float RefractionCoef;
            public int MaterialType;
            public Material(Vector3 color, Vector4 lightCoefs, float reflectionCoef, float refractionCoef, int type)
            {
                Color = color;
                LightCoeffs = lightCoefs;
                ReflectionCoef = reflectionCoef;
                RefractionCoef = refractionCoef;
                MaterialType = type;
            }
        };
    }
}

