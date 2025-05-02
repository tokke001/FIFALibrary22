
'Namespace FifaLibrary
Public Enum ECompressionMode
    ' Fields
    None = 0
    Compressed_10FB = 1
    Chunkzip = 2
    Chunkzip2 = 3
    Chunkref = 4
    Chunkref2 = 5
    EASF = 6 'EASF_FIFA16
    Chunklzma = 7   'chunlzma

    Chunklzx    'FIFA10 console
    Chunklzx2   'unknown newer, found at chunkpack tool
    Chunklz4
    Chunkunc
    Chunzstd
    Chunoodl    'switch

    Unknown


End Enum
'End Namespace

