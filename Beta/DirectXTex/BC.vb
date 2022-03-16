Imports Microsoft.DirectX
Public Class BC
	'-------------------------------------------------------------------------------------
	' Constants
	'-------------------------------------------------------------------------------------

	' Perceptual weightings for the importance of each channel.
	'C++ TO VB CONVERTER TODO TASK: The following statement was not recognized, possibly due to an unrecognized macro:
	Const HDRColorA g_Luminance(0.2125F / 0.7154F, 1.0F, 0.0721F / 0.7154F, 1.0F)
'C++ TO VB CONVERTER TODO TASK: The following statement was not recognized, possibly due to an unrecognized macro:
	Const HDRColorA g_LuminanceInv(0.7154F / 0.2125F, 1.0F, 0.7154F / 0.0721F, 1.0F)

	'-------------------------------------------------------------------------------------
	' Decode/Encode RGB 5/6/5 colors
	'-------------------------------------------------------------------------------------
'C++ TO VB CONVERTER TODO TASK: VB has no equivalent to 'noexcept':
'ORIGINAL LINE: inline void Decode565(_Out_ HDRColorA *pColor, _In_ const unsigned short w565) noexcept
	Private Sub Decode565(ByVal pColor As _Out_, ByVal w565 As _In_)
		pColor.r = CSng((w565 >> 11) And 31) * (1.0F / 31.0F)
		pColor.g = CSng((w565 >> 5) And 63) * (1.0F / 63.0F)
		pColor.b = CSng((w565 >> 0) And 31) * (1.0F / 31.0F)
		pColor.a = 1.0F
	End Sub

	'C++ TO VB CONVERTER TODO TASK: VB has no equivalent to 'noexcept':
	'ORIGINAL LINE: inline unsigned short Encode565(_In_ const HDRColorA *pColor) noexcept
	Private Function Encode565(ByVal pColor As _In_) As UShort
		Dim Color As New HDRColorA()

		Color.r = If(pColor.r < 0.0F, 0.0F, If(pColor.r > 1.0F, 1.0F, pColor.r))
		Color.g = If(pColor.g < 0.0F, 0.0F, If(pColor.g > 1.0F, 1.0F, pColor.g))
		Color.b = If(pColor.b < 0.0F, 0.0F, If(pColor.b > 1.0F, 1.0F, pColor.b))
		Color.a = pColor.a

		Dim w As UShort

		w = CUShort((CInt(Math.Truncate(Color.r * 31.0F + 0.5F)) << 11) Or (CInt(Math.Truncate(Color.g * 63.0F + 0.5F)) << 5) Or (CInt(Math.Truncate(Color.b * 31.0F + 0.5F)) << 0))

		Return w
	End Function

	'-------------------------------------------------------------------------------------
	'C++ TO VB CONVERTER TODO TASK: VB has no equivalent to 'noexcept':
	'ORIGINAL LINE: inline void DecodeBC1(_Out_writes_(NUM_PIXELS_PER_BLOCK) XMVECTOR *pColor, _In_ const D3DX_BC1 *pBC, bool isbc1) noexcept
	Private Sub DecodeBC1(ByVal pColor() As _Out_writes_, ByVal pBC As _In_, ByVal isbc1 As Boolean)
		Debug.Assert(pColor AndAlso pBC)
		'C++ TO VB CONVERTER TODO TASK: There is no equivalent in VB to 'static_assert':
		'		static_assert(sizeof(D3DX_BC1) == 8, "D3DX_BC1 should be 8 bytes");

		Static s_Scale As New XMVECTORF32({
			{1.0F / 31.0F, 1.0F / 63.0F, 1.0F / 31.0F, 1.0F}
		})

		'C++ TO VB CONVERTER TODO TASK: There is no equivalent to 'reinterpret_cast' in VB:
		Dim clr0 As XMVECTOR = XMLoadU565(reinterpret_cast <const XMU565*>(pBC.rgb(0)))
'C++ TO VB CONVERTER TODO TASK: There is no equivalent to 'reinterpret_cast' in VB:
		Dim clr1 As XMVECTOR = XMLoadU565(reinterpret_cast <const XMU565*>(pBC.rgb(1)))

		clr0 = XMVectorMultiply(clr0, s_Scale)
		clr1 = XMVectorMultiply(clr1, s_Scale)

		clr0 = XMVectorSwizzle < 2, 1, 0, 3>(clr0)
		clr1 = XMVectorSwizzle < 2, 1, 0, 3>(clr1)

		clr0 = XMVectorSelect(g_XMIdentityR3, clr0, g_XMSelect1110)
		clr1 = XMVectorSelect(g_XMIdentityR3, clr1, g_XMSelect1110)

		Dim clr2 As New XMVECTOR()
		Dim clr3 As New XMVECTOR()
		If isbc1 AndAlso (pBC.rgb(0) <= pBC.rgb(1)) Then
			clr2 = XMVectorLerp(clr0, clr1, 0.5F)
			clr3 = XMVectorZero() ' Alpha of 0
		Else
			clr2 = XMVectorLerp(clr0, clr1, 1.0F / 3.0F)
			clr3 = XMVectorLerp(clr0, clr1, 2.0F / 3.0F)
		End If

		Dim dw As UInteger = pBC.bitmap

		Dim i As UInteger = 0
		Do
			Select Case dw And 3
				Case 0
					pColor(i) = clr0
				Case 1
					pColor(i) = clr1
				Case 2
					pColor(i) = clr2

				Case Else
					pColor(i) = clr3
			End Select
		Loop
	End Sub


End Class
