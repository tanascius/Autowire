using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.CSharp;
using NUnit.Framework;

#pragma warning disable 162

namespace Autowire.Tests.Performance
{
	[SetUpFixture]
	public class SetUpFixture
	{
		public static Type[] DynamicTypes = Type.EmptyTypes;

		[SetUp]
		public void Setup()
		{
#if !DEBUG
			Assert.Fail( "Unittests can be executed in debug mode, only." );
#endif

			//if( DynamicTypes != Type.EmptyTypes )
			{
				return;
			}

			const int outerCount = 20;
			const int innerCount = 250;
			DynamicTypes = new Type[outerCount * innerCount];

			for( var outerIndex = 0; outerIndex < outerCount; outerIndex++ )
			{
				var codeNamespace = new CodeNamespace( "DynamicClasses" );
				codeNamespace.Imports.Add( new CodeNamespaceImport( "System" ) );
				codeNamespace.Imports.Add( new CodeNamespaceImport( "System.Windows.Forms" ) );

				for( var innerIndex = 0; innerIndex < innerCount; innerIndex++ )
				{
					var classToCreate = new CodeTypeDeclaration( "DynamicClass_" + ( outerIndex * innerCount + innerIndex ) )
					{
						TypeAttributes = TypeAttributes.Public
					};
					var codeConstructor1 = new CodeConstructor
					{
						Attributes = MemberAttributes.Public
					};
					classToCreate.Members.Add( codeConstructor1 );

					var codeConstructor2 = new CodeConstructor
					{
						Attributes = MemberAttributes.Public
					};
					codeConstructor2.Parameters.Add( new CodeParameterDeclarationExpression( typeof( int ), "a" ) );
					classToCreate.Members.Add( codeConstructor2 );

					var codeConstructor3 = new CodeConstructor
					{
						Attributes = MemberAttributes.Public
					};
					codeConstructor3.Parameters.Add( new CodeParameterDeclarationExpression( typeof( int ), "a" ) );
					codeConstructor3.Parameters.Add( new CodeParameterDeclarationExpression( typeof( int ), "b" ) );
					classToCreate.Members.Add( codeConstructor3 );

					var codeConstructor4 = new CodeConstructor
					{
						Attributes = MemberAttributes.Public
					};
					codeConstructor4.Parameters.Add( new CodeParameterDeclarationExpression( typeof( string ), "a" ) );
					classToCreate.Members.Add( codeConstructor4 );

					var codeConstructor5 = new CodeConstructor
					{
						Attributes = MemberAttributes.Public
					};
					codeConstructor5.Parameters.Add( new CodeParameterDeclarationExpression( typeof( Form ), "a" ) );
					codeConstructor5.Parameters.Add( new CodeParameterDeclarationExpression( typeof( Control ), "b" ) );
					codeConstructor5.Parameters.Add( new CodeParameterDeclarationExpression( typeof( Timer ), "c" ) );
					codeConstructor5.Parameters.Add( new CodeParameterDeclarationExpression( typeof( Button ), "d" ) );
					classToCreate.Members.Add( codeConstructor5 );

					codeNamespace.Types.Add( classToCreate );
				}

				var codeCompileUnit = new CodeCompileUnit();
				codeCompileUnit.Namespaces.Add( codeNamespace );

				var compilerParameters = new CompilerParameters
				{
					GenerateInMemory = true,
					IncludeDebugInformation = true,
					TreatWarningsAsErrors = true,
					WarningLevel = 4
				};
				compilerParameters.ReferencedAssemblies.Add( "System.dll" );
				compilerParameters.ReferencedAssemblies.Add( "System.Windows.Forms.dll" );

				var compilerResults = new CSharpCodeProvider().CompileAssemblyFromDom( compilerParameters, codeCompileUnit );

				if( compilerResults == null )
				{
					throw new InvalidOperationException( "ClassCompiler did not return results." );
				}
				if( compilerResults.Errors.HasErrors )
				{
					var errors = string.Empty;
					foreach( CompilerError compilerError in compilerResults.Errors )
					{
						errors += compilerError.ErrorText + "\n";
					}
					Debug.Fail( errors );
					throw new InvalidOperationException( "Errors while compiling the dynamic classes:\n" + errors );
				}

				var dynamicAssembly = compilerResults.CompiledAssembly;
				dynamicAssembly.GetExportedTypes().CopyTo( DynamicTypes, outerIndex * innerCount );
			}
		}
	}
}

#pragma warning restore 162