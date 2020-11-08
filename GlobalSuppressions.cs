// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.

// Yes, I know I'd usually put a capital letter at the start of my class names,
// but 'fishBytes' is meant to start with a lowercase letter.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>", Scope = "type", Target = "~T:fishBytesInterpreter")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>", Scope = "type", Target = "~T:fishBytesNativeObject")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>", Scope = "type", Target = "~T:fishBytesFunction")]

// No, I don't care about StringComparison culture garbage.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Common Practices and Code Improvements", "RECS0063:Warns when a culture-aware 'StartsWith' call is used by default.", Justification = "<Pending>", Scope = "member", Target = "~M:fishBytesNativeObject.GetStructFromString(System.String)~System.Object")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Common Practices and Code Improvements", "RECS0061:Warns when a culture-aware 'EndsWith' call is used by default.", Justification = "<Pending>", Scope = "member", Target = "~M:fishBytesNativeObject.GetStructFromString(System.String)~System.Object")]
