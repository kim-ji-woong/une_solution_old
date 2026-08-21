Shader "Simple Additive"
{
  Properties
  {
    _MainTex ("Texture to blend", 2D) = "red" {}
  }

  SubShader
  {
    Tags { "Queue" = "Transparent" }
    Pass
    {
      Blend One One
      SetTexture [_MainTex] { combine texture }
    }
  }
}
