using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee.SpreadLedger
{
    public class ClassParse
    {
        public static void DoSomething()
        {
            var source = @"
                public class DoSomething
                {
                    public static string WaHey(string something)
                    {
                        return something;
                    }
                }
            ";

            //Evidence ev = new Evidence();
            //ev.AddHostEvidence(new Zone(SecurityZone.Internet));
            //PermissionSet permSet = SecurityManager.GetStandardSandbox(ev);

            //StrongName fullTrustAssembly = typeof(Program).Assembly.Evidence.GetHostEvidence<StrongName>();

            //AppDomainSetup adSetup = new AppDomainSetup()
            //{
            //    ApplicationBase = Path.GetFullPath(Environment.CurrentDirectory)
            //};

            //AppDomain newDomain = AppDomain.CreateDomain("Sandbox", ev, adSetup, permSet, fullTrustAssembly);

            //Assembly asm = newDomain.Load(System.IO.File.ReadAllBytes("ExternalLib.dll"));
            //var instance = asm.CreateInstance("ExternalLib.MyProvider", true);

            //IProvider provider = instance as IProvider;

            //Should not work, because my Assembly is accessing a file/Registry or something else
            //string result = provider.Run("Test");


            //PermissionSet permSet = new PermissionSet(PermissionState.None);
            //permSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            //permSet.RemovePermission(typeof(FileIOPermission));
            //permSet.RemovePermission(typeof(RegistryPermission));

            //var syntaxTree = CSharpSyntaxTree.ParseText(source);

            ////We first have to choose what kind of output we're creating: DLL, .exe etc.
            //var options = new CSharpCompilationOptions(OutputKind.ConsoleApplication);
            //options = options.WithAllowUnsafe(false);                                //Allow unsafe code;
            //options = options.WithOptimizationLevel(OptimizationLevel.Release);     //Set optimization level
            //options = options.WithPlatform(Platform.AnyCpu);                         //Set platform

            //CSharpCompilation compilation = CSharpCompilation.Create(
            //    "assemblyName",
            //    new[] { syntaxTree },
            //    new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
            //    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            //Assembly assembly = null;

            //using (var dllStream = new MemoryStream())
            //using (var pdbStream = new MemoryStream())
            //{
            //    var emitResult = compilation.Emit(dllStream, pdbStream);
            //    if (!emitResult.Success)
            //    {
            //        // emitResult.Diagnostics
            //    }
            //    else
            //    {
            //        dllStream.Seek(0, SeekOrigin.Begin);
            //        pdbStream.Seek(0, SeekOrigin.Begin);
            //        assembly = Assembly.Load(dllStream.ToArray(), pdbStream.ToArray(), new Evidence(){ );
            //    }
            //}

            //var type = assembly.GetType("DoSomething");

            //type.InvokeMember("Main", BindingFlags.Default | BindingFlags.InvokeMethod, null, null, null);
        }
    }
}
