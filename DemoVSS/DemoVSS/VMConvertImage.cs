using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
//using static VM.PlatformSDKCS.ImvsSdkDefine;
using OpenCvSharp;
using VM.PlatformSDKCS;
using MvCamCtrl.NET;
using VisionDesigner;

namespace DemoVSS
{
    public static class VMConvertImage
    {
        public static ImageBaseData_V2 ToImageBaseData_V2(this Bitmap bmpInputImg)
        {
            var img = bmpInputImg.ToMVDImage();
            ImageBaseData_V2 imageBaseData_V2 = img.CMvdImageToImageBaseData_V2();
            return imageBaseData_V2;
        }

        public static ImageBaseData_V2 ToImageBaseData_V2(this Mat cBitmapImg)
        {
            return cBitmapImg.ToMVDImage().CMvdImageToImageBaseData_V2();
        }

        public static ImageBaseData_V2 CMvdImageToImageBaseData_V2(this CMvdImage cmvdImage)
        {
            VM.PlatformSDKCS.ImageBaseData_V2 ImageBaseDataV2 = null;
            if (MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08 == cmvdImage.PixelFormat)
            {
                var cmvdImageData = cmvdImage.GetImageData();
                IntPtr imagedata = Marshal.AllocHGlobal(cmvdImageData.stDataChannel[0].arrDataBytes.Length);
                Marshal.Copy(cmvdImageData.stDataChannel[0].arrDataBytes, 0, imagedata, cmvdImageData.stDataChannel[0].arrDataBytes.Length);
                ImageBaseDataV2 = new ImageBaseData_V2(imagedata, (uint)cmvdImageData.stDataChannel[0].arrDataBytes.Length, (int)cmvdImage.Width, (int)cmvdImage.Height, VMPixelFormat.VM_PIXEL_MONO_08);
                cmvdImageData = null;
                //Marshal.FreeHGlobal(imagedata);
                imagedata = IntPtr.Zero;
            }
            else if (MVD_PIXEL_FORMAT.MVD_PIXEL_RGB_RGB24_C3 == cmvdImage.PixelFormat)
            {

                var cmvdImageData = cmvdImage.GetImageData();
                IntPtr imagedata = Marshal.AllocHGlobal(cmvdImageData.stDataChannel[0].arrDataBytes.Length);
                Marshal.Copy(cmvdImageData.stDataChannel[0].arrDataBytes, 0, imagedata, cmvdImageData.stDataChannel[0].arrDataBytes.Length);
                ImageBaseDataV2 = new ImageBaseData_V2(imagedata, (uint)cmvdImageData.stDataChannel[0].arrDataBytes.Length, (int)cmvdImage.Width, (int)cmvdImage.Height, VMPixelFormat.VM_PIXEL_RGB24_C3);
                cmvdImageData = null;
                //Marshal.FreeHGlobal(imagedata);
                imagedata = IntPtr.Zero;
            }

            return ImageBaseDataV2;
        }

        public static ImageBaseData_V2 LoadImage(this CMvdImage cmvdImage, string pathfileimage, bool isColor)
        {

            var img = new CMvdImage();
            if (isColor)
            {
                img.InitImage(pathfileimage);
            }
            else
            {
                img.InitImage(pathfileimage, MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08);
            }

            var img1 = img.CMvdImageToImageBaseData_V2();
            return img1;
        }

        /// <summary>
        /// 图像转换：Bitmap -> CMvdImage
        /// </summary>
        /// <param name="cHaclonImg">[IN]Bitmap图像</param>
        /// <param name="cMvdImage">[INOUT]CMvdImage图像</param>
        /// <remarks>只支持Mon8和RGB24，不支持伪彩图</remarks>
        private static void ConvertBitmap2MVDImage(this Bitmap cBitmapImg, CMvdImage cMvdImg)
        {
            // 参数合法性判断
            if (null == cBitmapImg || null == cMvdImg)
            {
                throw new MvdException(MVD_MODULE_TYPE.MVD_MODUL_APP, MVD_ERROR_CODE.MVD_E_PARAMETER_ILLEGAL);
            }

            // 判断像素格式
            if (PixelFormat.Format8bppIndexed != cBitmapImg.PixelFormat && PixelFormat.Format24bppRgb != cBitmapImg.PixelFormat)
            {
                throw new MvdException(MVD_MODULE_TYPE.MVD_MODUL_APP, MVD_ERROR_CODE.MVD_E_SUPPORT);
            }

            Int32 nImageWidth = cBitmapImg.Width;
            Int32 nImageHeight = cBitmapImg.Height;
            Int32 nChannelNum = 0;
            BitmapData bitmapData = null;

            try
            {
                // 获取图像信息
                if (PixelFormat.Format8bppIndexed == cBitmapImg.PixelFormat) // 灰度图
                {
                    bitmapData = cBitmapImg.LockBits(new Rectangle(0, 0, nImageWidth, nImageHeight)
                    , ImageLockMode.ReadOnly
                                                                 , PixelFormat.Format8bppIndexed);
                    cMvdImg.InitImage(Convert.ToUInt32(nImageWidth), Convert.ToUInt32(nImageHeight), MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08);
                    nChannelNum = 1;
                }
                else if (PixelFormat.Format24bppRgb == cBitmapImg.PixelFormat) // 彩色图
                {
                    bitmapData = cBitmapImg.LockBits(new Rectangle(0, 0, nImageWidth, nImageHeight)
                    , ImageLockMode.ReadOnly
                                                                , PixelFormat.Format24bppRgb);
                    cMvdImg.InitImage(Convert.ToUInt32(nImageWidth), Convert.ToUInt32(nImageHeight), MVD_PIXEL_FORMAT.MVD_PIXEL_RGB_RGB24_C3);
                    nChannelNum = 3;
                }

                // 考虑图像是否4字节对齐，bitmap要求4字节对齐，而mvdimage不要求对齐
                if (0 == nImageWidth % 4) // 4字节对齐时，直接拷贝
                {
                    Marshal.Copy(bitmapData.Scan0, cMvdImg.GetImageData().stDataChannel[0].arrDataBytes, 0, nImageWidth * nImageHeight * nChannelNum);
                }
                else // 按步长逐行拷贝
                {
                    // 每行实际占用字节数
                    Int32 nRowPixelByteNum = nImageWidth * nChannelNum + 4 - (nImageWidth * nChannelNum % 4);
                    // 每行首字节首地址
                    IntPtr bitmapDataRowPos = IntPtr.Zero;
                    for (int i = 0; i < nImageHeight; i++)
                    {
                        // 获取每行第一个像素值的首地址
                        bitmapDataRowPos = new IntPtr(bitmapData.Scan0.ToInt64() + nRowPixelByteNum * i);
                        Marshal.Copy(bitmapDataRowPos, cMvdImg.GetImageData().stDataChannel[0].arrDataBytes, i * nImageWidth * nChannelNum, nImageWidth * nChannelNum);
                    }
                }

                // bitmap彩色图按BGR存储，而MVDimg按RGB存储，改变存储顺序
                // 交换R和B
                if (PixelFormat.Format24bppRgb == cBitmapImg.PixelFormat)
                {
                    byte bTemp;
                    byte[] bMvdImgData = cMvdImg.GetImageData().stDataChannel[0].arrDataBytes;
                    for (int i = 0; i < nImageWidth * nImageHeight; i++)
                    {
                        bTemp = bMvdImgData[3 * i];
                        bMvdImgData[3 * i] = bMvdImgData[3 * i + 2];
                        bMvdImgData[3 * i + 2] = bTemp;
                    }
                }
            }
            finally
            {
                cBitmapImg.UnlockBits(bitmapData);
            }
        }

        public static CMvdImage ToMVDImage(this Bitmap cBitmapImg)
        {
            CMvdImage cMvdImg = new CMvdImage();
            // 参数合法性判断
            if (null == cBitmapImg)
            {
                throw new MvdException(MVD_MODULE_TYPE.MVD_MODUL_APP, VisionDesigner.MVD_ERROR_CODE.MVD_E_PARAMETER_ILLEGAL);
            }

            // 判断像素格式
            if (PixelFormat.Format8bppIndexed != cBitmapImg.PixelFormat && PixelFormat.Format24bppRgb != cBitmapImg.PixelFormat)
            {
                throw new MvdException(MVD_MODULE_TYPE.MVD_MODUL_APP, VisionDesigner.MVD_ERROR_CODE.MVD_E_SUPPORT);
            }

            Int32 nImageWidth = cBitmapImg.Width;
            Int32 nImageHeight = cBitmapImg.Height;
            Int32 nChannelNum = 0;
            BitmapData bitmapData = null;

            try
            {
                // 获取图像信息
                if (PixelFormat.Format8bppIndexed == cBitmapImg.PixelFormat) // 灰度图
                {
                    bitmapData = cBitmapImg.LockBits(new Rectangle(0, 0, nImageWidth, nImageHeight)
                    , ImageLockMode.ReadOnly
                                                                 , PixelFormat.Format8bppIndexed);
                    cMvdImg.InitImage(Convert.ToUInt32(nImageWidth), Convert.ToUInt32(nImageHeight), MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08);
                    nChannelNum = 1;
                }
                else if (PixelFormat.Format24bppRgb == cBitmapImg.PixelFormat) // 彩色图
                {
                    bitmapData = cBitmapImg.LockBits(new Rectangle(0, 0, nImageWidth, nImageHeight)
                                                                , ImageLockMode.ReadOnly
                                                                , PixelFormat.Format24bppRgb);
                    cMvdImg.InitImage(Convert.ToUInt32(nImageWidth), Convert.ToUInt32(nImageHeight), MVD_PIXEL_FORMAT.MVD_PIXEL_RGB_RGB24_C3);
                    nChannelNum = 3;
                }

                // 考虑图像是否4字节对齐，bitmap要求4字节对齐，而mvdimage不要求对齐
                if (0 == nImageWidth % 4) // 4字节对齐时，直接拷贝
                {
                    Marshal.Copy(bitmapData.Scan0, cMvdImg.GetImageData().stDataChannel[0].arrDataBytes, 0, nImageWidth * nImageHeight * nChannelNum);
                }
                else // 按步长逐行拷贝
                {
                    // 每行实际占用字节数
                    Int32 nRowPixelByteNum = nImageWidth * nChannelNum + 4 - (nImageWidth * nChannelNum % 4);
                    // 每行首字节首地址
                    IntPtr bitmapDataRowPos = IntPtr.Zero;
                    for (int i = 0; i < nImageHeight; i++)
                    {
                        // 获取每行第一个像素值的首地址
                        bitmapDataRowPos = new IntPtr(bitmapData.Scan0.ToInt64() + nRowPixelByteNum * i);
                        Marshal.Copy(bitmapDataRowPos, cMvdImg.GetImageData().stDataChannel[0].arrDataBytes, i * nImageWidth * nChannelNum, nImageWidth * nChannelNum);
                    }
                }

                // bitmap彩色图按BGR存储，而MVDimg按RGB存储，改变存储顺序
                // 交换R和B
                if (PixelFormat.Format24bppRgb == cBitmapImg.PixelFormat)
                {
                    byte bTemp;
                    byte[] bMvdImgData = cMvdImg.GetImageData().stDataChannel[0].arrDataBytes;
                    for (int i = 0; i < nImageWidth * nImageHeight; i++)
                    {
                        bTemp = bMvdImgData[3 * i];
                        bMvdImgData[3 * i] = bMvdImgData[3 * i + 2];
                        bMvdImgData[3 * i + 2] = bTemp;
                    }
                }

                return cMvdImg;
            }
            finally
            {
                cBitmapImg.UnlockBits(bitmapData);

                GC.Collect();

            }

        }

        /// <summary>
        /// 图像转换：CMvdImage -> Bitmap
        /// </summary>
        /// <param name="cHaclonImg">[IN]CMvdImage图像</param>
        /// <param name="cMvdImage">[INOUT]Bitmap图像</param>
        /// <remarks>只支持Mon8和RGB24，不支持伪彩图</remarks>
        public static void ConvertMVDImage2Bitmap(this CMvdImage cMvdImg, ref Bitmap cBitmapImg)
        {
            // 参数合法性判断
            if (null == cMvdImg)
            {
                throw new MvdException(MVD_MODULE_TYPE.MVD_MODUL_APP, VisionDesigner.MVD_ERROR_CODE.MVD_E_PARAMETER_ILLEGAL);
            }

            // 判断像素格式
            if (MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08 != cMvdImg.PixelFormat && MVD_PIXEL_FORMAT.MVD_PIXEL_RGB_RGB24_C3 != cMvdImg.PixelFormat)
            {
                throw new MvdException(MVD_MODULE_TYPE.MVD_MODUL_APP, VisionDesigner.MVD_ERROR_CODE.MVD_E_SUPPORT);
            }

            Int32 nImageWidth = Convert.ToInt32(cMvdImg.Width);
            Int32 nImageHeight = Convert.ToInt32(cMvdImg.Height);
            Int32 nChannelNum = 0;
            BitmapData bitmapData = null;
            byte[] bBitmapDataTemp = null;
            try
            {
                // 获取图像信息
                if (MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08 == cMvdImg.PixelFormat) // 灰度图
                {
                    cBitmapImg = new Bitmap(nImageWidth, nImageHeight, PixelFormat.Format8bppIndexed);

                    // 灰度图需指定调色板
                    ColorPalette colorPalette = cBitmapImg.Palette;
                    for (int j = 0; j < 256; j++)
                    {
                        colorPalette.Entries[j] = Color.FromArgb(j, j, j);
                    }
                    cBitmapImg.Palette = colorPalette;

                    bitmapData = cBitmapImg.LockBits(new Rectangle(0, 0, nImageWidth, nImageHeight)
                                                                 , ImageLockMode.WriteOnly
                                                                 , PixelFormat.Format8bppIndexed);

                    // 灰度图不做深拷贝
                    bBitmapDataTemp = cMvdImg.GetImageData().stDataChannel[0].arrDataBytes;
                    nChannelNum = 1;
                }
                else if (MVD_PIXEL_FORMAT.MVD_PIXEL_RGB_RGB24_C3 == cMvdImg.PixelFormat) // 彩色图
                {
                    cBitmapImg = new Bitmap(nImageWidth, nImageHeight, PixelFormat.Format24bppRgb);
                    bitmapData = cBitmapImg.LockBits(new Rectangle(0, 0, nImageWidth, nImageHeight)
                                                                , ImageLockMode.WriteOnly
                                                                , PixelFormat.Format24bppRgb);
                    // 彩色图做深拷贝
                    bBitmapDataTemp = new byte[cMvdImg.GetImageData().stDataChannel[0].arrDataBytes.Length];
                    Array.Copy(cMvdImg.GetImageData().stDataChannel[0].arrDataBytes, bBitmapDataTemp, bBitmapDataTemp.Length);
                    nChannelNum = 3;
                }

                // bitmap彩色图按BGR存储，而MVDimg按RGB存储，改变存储顺序
                // 交换R和B
                if (MVD_PIXEL_FORMAT.MVD_PIXEL_RGB_RGB24_C3 == cMvdImg.PixelFormat)
                {
                    byte bTemp;
                    for (int i = 0; i < nImageWidth * nImageHeight; i++)
                    {
                        bTemp = bBitmapDataTemp[3 * i];
                        bBitmapDataTemp[3 * i] = bBitmapDataTemp[3 * i + 2];
                        bBitmapDataTemp[3 * i + 2] = bTemp;
                    }
                }

                // 考虑图像是否4字节对齐，bitmap要求4字节对齐，而mvdimage不要求对齐
                if (0 == nImageWidth % 4) // 4字节对齐时，直接拷贝
                {
                    Marshal.Copy(bBitmapDataTemp, 0, bitmapData.Scan0, nImageWidth * nImageHeight * nChannelNum);
                }
                else // 按步长逐行拷贝
                {
                    // 每行实际占用字节数
                    Int32 nRowPixelByteNum = nImageWidth * nChannelNum + 4 - (nImageWidth * nChannelNum % 4);
                    // 每行首字节首地址
                    IntPtr bitmapDataRowPos = IntPtr.Zero;
                    for (int i = 0; i < nImageHeight; i++)
                    {
                        // 获取每行第一个像素值的首地址
                        bitmapDataRowPos = new IntPtr(bitmapData.Scan0.ToInt64() + nRowPixelByteNum * i);
                        Marshal.Copy(bBitmapDataTemp, i * nImageWidth * nChannelNum, bitmapDataRowPos, nImageWidth * nChannelNum);
                    }
                }

                cBitmapImg.UnlockBits(bitmapData);
            }
            catch (MvdException ex)
            {
                if (null != cBitmapImg)
                {
                    cBitmapImg.UnlockBits(bitmapData);
                    cBitmapImg.Dispose();
                    cBitmapImg = null;
                }
                throw ex;
            }
            catch (System.Exception ex)
            {
                if (null != cBitmapImg)
                {
                    cBitmapImg.UnlockBits(bitmapData);
                    cBitmapImg.Dispose();
                    cBitmapImg = null;
                }
                throw ex;
            }
        }

        public static void GetImvdImgFromCamerData(UInt32 nWidth, UInt32 nHeight, Byte[] byteGrapData, MyCamera.MvGvspPixelType enPixelType, CMvdImage cMvdImage)
        {
            if (0 == nWidth || 0 == nHeight || null == byteGrapData || null == cMvdImage)
            {
                throw new MvdException(MVD_MODULE_TYPE.MVD_MODUL_APP, VisionDesigner.MVD_ERROR_CODE.MVD_E_PARAMETER_ILLEGAL);
            }
            if ((MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8 != enPixelType) && (MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed != enPixelType))
            {
                throw new MvdException(MVD_MODULE_TYPE.MVD_MODUL_APP, VisionDesigner.MVD_ERROR_CODE.MVD_E_SUPPORT);
            }
            MVD_IMAGE_DATA_INFO imagedate = new MVD_IMAGE_DATA_INFO();

            // 根据出图宽高初始化图像
            if (MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8 == enPixelType)
            {
                imagedate.stDataChannel[0].arrDataBytes = byteGrapData;
                imagedate.stDataChannel[0].nLen = nWidth * nHeight;
                imagedate.stDataChannel[0].nSize = nWidth * nHeight;
                imagedate.stDataChannel[0].nRowStep = nWidth;
                cMvdImage.InitImage(nWidth, nHeight, MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08, imagedate);
            }
            else if (MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed == enPixelType)
            {
                imagedate.stDataChannel[0].arrDataBytes = byteGrapData;
                imagedate.stDataChannel[0].nLen = nWidth * nHeight * 3;
                imagedate.stDataChannel[0].nSize = nWidth * nHeight * 3;
                imagedate.stDataChannel[0].nRowStep = nWidth * 3;
                cMvdImage.InitImage(nWidth, nHeight, MVD_PIXEL_FORMAT.MVD_PIXEL_RGB_RGB24_C3, imagedate);
            }
        }

        /// <summary>
        /// 图像转换：Mat -> CMvdImage
        /// </summary>
        /// <param name="mat">[IN]Mat图像</param>
        /// <param name="cMvdImage">[INOUT]CMvdImage图像</param>
        private static void ConvertMat2MVDImage(this Mat mat, CMvdImage cMvdImg)
        {
            // 参数合法性判断
            if (null == mat || null == cMvdImg)
            {
                throw new MvdException(MVD_MODULE_TYPE.MVD_MODUL_APP, VisionDesigner.MVD_ERROR_CODE.MVD_E_HANDLE);
            }
            // 像素格式判断
            if (MatType.CV_8UC1 != mat.Type() && MatType.CV_8UC3 != mat.Type())
            {
                throw new MvdException(MVD_MODULE_TYPE.MVD_MODUL_APP, VisionDesigner.MVD_ERROR_CODE.MVD_E_SUPPORT);
            }

            uint imgWidth = (uint)mat.Size().Width;// 图片的真实宽度
            uint imgHeight = (uint)mat.Size().Height;// 图片的真实高度
            byte[] bMvdImgData = null;

            // 根据传入的mat图像初始化MVDImage，并进行转换
            if (mat.Type() == MatType.CV_8UC1)
            {
                cMvdImg.InitImage(imgWidth, imgHeight, MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08);
                bMvdImgData = cMvdImg.GetImageData().stDataChannel[0].arrDataBytes;
                Marshal.Copy(mat.Ptr(0), bMvdImgData, 0, bMvdImgData.Length);
            }
            else if (mat.Type() == MatType.CV_8UC3)
            {
                cMvdImg.InitImage(imgWidth, imgHeight, MVD_PIXEL_FORMAT.MVD_PIXEL_RGB_RGB24_C3);
                bMvdImgData = cMvdImg.GetImageData().stDataChannel[0].arrDataBytes;
                Marshal.Copy(mat.Ptr(0), bMvdImgData, 0, bMvdImgData.Length);

                // Mat为BGRBGR...存储，MVDImage为RGBRGB...存储，需要调整
                byte bTemp;
                for (int i = 0; i < imgWidth * imgHeight; i++)
                {
                    bTemp = bMvdImgData[3 * i];
                    bMvdImgData[3 * i] = bMvdImgData[3 * i + 2];
                    bMvdImgData[3 * i + 2] = bTemp;
                }
            }
        }

        private static CMvdImage ToMVDImage(this Mat mat)
        {
            CMvdImage cMvdImg = new CMvdImage();
            // 参数合法性判断
            if (null == mat)
            {
                throw new MvdException(MVD_MODULE_TYPE.MVD_MODUL_APP, VisionDesigner.MVD_ERROR_CODE.MVD_E_HANDLE);
            }
            // 像素格式判断
            if (MatType.CV_8UC1 != mat.Type() && MatType.CV_8UC3 != mat.Type())
            {
                throw new MvdException(MVD_MODULE_TYPE.MVD_MODUL_APP, VisionDesigner.MVD_ERROR_CODE.MVD_E_SUPPORT);
            }

            uint imgWidth = (uint)mat.Size().Width;// 图片的真实宽度
            uint imgHeight = (uint)mat.Size().Height;// 图片的真实高度
            byte[] bMvdImgData = null;

            // 根据传入的mat图像初始化MVDImage，并进行转换
            if (mat.Type() == MatType.CV_8UC1)
            {
                cMvdImg.InitImage(imgWidth, imgHeight, MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08);
                bMvdImgData = cMvdImg.GetImageData().stDataChannel[0].arrDataBytes;
                Marshal.Copy(mat.Ptr(0), bMvdImgData, 0, bMvdImgData.Length);
            }
            else if (mat.Type() == MatType.CV_8UC3)
            {
                cMvdImg.InitImage(imgWidth, imgHeight, MVD_PIXEL_FORMAT.MVD_PIXEL_RGB_RGB24_C3);
                bMvdImgData = cMvdImg.GetImageData().stDataChannel[0].arrDataBytes;
                Marshal.Copy(mat.Ptr(0), bMvdImgData, 0, bMvdImgData.Length);

                // Mat为BGRBGR...存储，MVDImage为RGBRGB...存储，需要调整
                byte bTemp;
                for (int i = 0; i < imgWidth * imgHeight; i++)
                {
                    bTemp = bMvdImgData[3 * i];
                    bMvdImgData[3 * i] = bMvdImgData[3 * i + 2];
                    bMvdImgData[3 * i + 2] = bTemp;
                }
            }
            return cMvdImg;
        }

        /// <summary>
        /// 图像转换：CMvdImage -> Mat
        /// </summary>
        /// <param name="mvdImage">[IN]CMvdImage图像</param>
        /// <param name="mat">[INOUT]Mat图像</param>
        private static void ConvertMVDImage2Mat(this CMvdImage mvdImage, Mat mat)
        {
            // 参数合法性判断
            if (null == mat || null == mvdImage)
            {
                throw new MvdException(MVD_MODULE_TYPE.MVD_MODUL_APP, VisionDesigner.MVD_ERROR_CODE.MVD_E_HANDLE);
            }

            // 像素格式判断
            if (mvdImage.PixelFormat != MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08 && mvdImage.PixelFormat != MVD_PIXEL_FORMAT.MVD_PIXEL_RGB_RGB24_C3)
            {
                throw new MvdException(MVD_MODULE_TYPE.MVD_MODUL_APP, VisionDesigner.MVD_ERROR_CODE.MVD_E_SUPPORT);
            }

            int imgWidth = (int)mvdImage.Width;
            int imgHeight = (int)mvdImage.Height;

            // 根据传入的MVDImage类型初始化Mat
            if (mvdImage.PixelFormat == MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08)
            {
                mat.Create(imgHeight, imgWidth, MatType.CV_8UC1);
                Marshal.Copy(mvdImage.GetImageData(0).arrDataBytes, 0, mat.Ptr(0), mvdImage.GetImageData(0).arrDataBytes.Length);
            }
            else if (mvdImage.PixelFormat == MVD_PIXEL_FORMAT.MVD_PIXEL_RGB_RGB24_C3)
            {
                mat.Create(imgHeight, imgWidth, MatType.CV_8UC3);
                // 先备份MVD图像数据，保证不改变源图像数据
                byte[] bMvdImgDataTemp = new byte[mvdImage.GetImageData(0).arrDataBytes.Length];
                Array.Copy(mvdImage.GetImageData(0).arrDataBytes, bMvdImgDataTemp, bMvdImgDataTemp.Length);

                // Mat为BGRBGR...存储，MVD为RGBRGB...存储，需要调整
                byte bTemp;
                for (int i = 0; i < imgWidth * imgHeight; i++)
                {
                    bTemp = bMvdImgDataTemp[3 * i];
                    bMvdImgDataTemp[3 * i] = bMvdImgDataTemp[3 * i + 2];
                    bMvdImgDataTemp[3 * i + 2] = bTemp;
                }
                // 将数据拷贝至Mat图像
                Marshal.Copy(bMvdImgDataTemp, 0, mat.Ptr(0), bMvdImgDataTemp.Length);
            }

        }


        private static Mat ToMat(this CMvdImage mvdImage)
        {
            Mat mat = null;
            // 参数合法性判断
            if (null == mvdImage)
            {
                throw new MvdException(MVD_MODULE_TYPE.MVD_MODUL_APP, VisionDesigner.MVD_ERROR_CODE.MVD_E_HANDLE);
            }

            // 像素格式判断
            if (mvdImage.PixelFormat != MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08 && mvdImage.PixelFormat != MVD_PIXEL_FORMAT.MVD_PIXEL_RGB_RGB24_C3)
            {
                throw new MvdException(MVD_MODULE_TYPE.MVD_MODUL_APP, VisionDesigner.MVD_ERROR_CODE.MVD_E_SUPPORT);
            }

            int imgWidth = (int)mvdImage.Width;
            int imgHeight = (int)mvdImage.Height;

            // 根据传入的MVDImage类型初始化Mat
            if (mvdImage.PixelFormat == MVD_PIXEL_FORMAT.MVD_PIXEL_MONO_08)
            {
                mat.Create(imgHeight, imgWidth, MatType.CV_8UC1);
                Marshal.Copy(mvdImage.GetImageData(0).arrDataBytes, 0, mat.Ptr(0), mvdImage.GetImageData(0).arrDataBytes.Length);
            }
            else if (mvdImage.PixelFormat == MVD_PIXEL_FORMAT.MVD_PIXEL_RGB_RGB24_C3)
            {
                mat.Create(imgHeight, imgWidth, MatType.CV_8UC3);
                // 先备份MVD图像数据，保证不改变源图像数据
                byte[] bMvdImgDataTemp = new byte[mvdImage.GetImageData(0).arrDataBytes.Length];
                Array.Copy(mvdImage.GetImageData(0).arrDataBytes, bMvdImgDataTemp, bMvdImgDataTemp.Length);

                // Mat为BGRBGR...存储，MVD为RGBRGB...存储，需要调整
                byte bTemp;
                for (int i = 0; i < imgWidth * imgHeight; i++)
                {
                    bTemp = bMvdImgDataTemp[3 * i];
                    bMvdImgDataTemp[3 * i] = bMvdImgDataTemp[3 * i + 2];
                    bMvdImgDataTemp[3 * i + 2] = bTemp;
                }
                // 将数据拷贝至Mat图像
                Marshal.Copy(bMvdImgDataTemp, 0, mat.Ptr(0), bMvdImgDataTemp.Length);
            }
            return mat;
        }
    }


}
