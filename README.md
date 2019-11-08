# SharpTracerCore
A ray tracer inspired by Peter Shirley's In One Weekend series

# Chapter 1
Chapter one adds a very basic method of putting some colors into an image format so we:

- Create a very, very basic Color class (RenderDataStructures\Color.cs)
- Create a very simple loop to calculate colors based on image space (RenderHandler\Renderer.cs -> Render() method)

Additionally, I build the basic framework of our application:

- Create a simple WPF GUI with MVVM databindings (SharpTracerCore_GUI\ViewModels\MainWindowViewModel.cs bound to SharpTracerCore_GUI\Views\MainWindowView.xaml)
- Create a simple CLI using Fluent Command Line Parser to get command line arguments for rendering (SharpTracerCore_CLI)
- Create a RenderParameters class to store render settings rather than hard coding them into our Render() method (RenderHandler\RenderParameters.cs)
- Add some nice quality-of-life additions, such as optional image watermarks and a render timer, and automatic opening of the rendered image upon completion
