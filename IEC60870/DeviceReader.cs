using System;
using System.Collections.Generic;

namespace IEC60870Driver
{
    public class DeviceReader
    {
        #region FIELDS 

        private int readTimes;
        private readonly ATDriver driver;
        private readonly List<BlockReader> blockReaders;
        private ClientAdapter clientAdapter;

        #endregion

        #region PROPERTIES

        public string DeviceName { get; set; }
        public string DeviceID { get; set; }
        public DeviceSettings Settings { get; set; }

        /// <summary>
        /// Trang thai ket noi
        /// </summary>
        public bool ConnectionStatus { get; private set; }

        #endregion

        public DeviceReader(ATDriver driver)
        {
            this.driver = driver;
            this.blockReaders = new List<BlockReader>();
        }

        /// <summary>
        /// Khoi tao ban dau. Tao danh sach cac Block can doc Multi
        /// </summary>
        public void Initialize()
        {
            if (Settings is null) return;
            this.clientAdapter = this.driver.TryGetClientAdapter(Settings.ClientID, out ClientAdapter adapter) ?
                adapter : this.driver.AddClientAdapter(Settings);

            if (string.IsNullOrEmpty(Settings.BlockSettings)) return;
            this.blockReaders.Clear();
            foreach (var blockSetting in Settings.BlockSettings.Split('/'))
                if (!string.IsNullOrEmpty(blockSetting.Trim()))
                    this.blockReaders.AddRange(BlockReader.Initialize(blockSetting));

            this.readTimes = Settings.MaxReadTimes;
        }

        public bool CheckConnection()
        {
            ConnectionStatus = this.clientAdapter != null && this.clientAdapter.CheckConnection();
            return ConnectionStatus;
        }

        /// <summary>
        /// Ham doc multi
        /// </summary>
        public void ReadMulti()
        {
            if (this.blockReaders.Count == 0) return;
            this.readTimes++;

            // Ap dung voi co che doc Multi
            // Khi doc Multi. Du lieu se duoc luu vao trong Buffer
            // Moi lan read Tag. Se duoc lay data tu Buffer (neu co).
            // Sau MaxReadTimes lan. Se doc lai Multi 1 lan de cap nhat gia tri moi
            if (this.readTimes <= Settings.MaxReadTimes) return;
            this.readTimes = 1;

            // Doc tuan tu cac Block
            // Ket qua tra ve se duoc luu vao Buffer
            foreach (var blockReader in this.blockReaders)
            {
                if (ConnectionStatus)
                    blockReader.ReadBlock(this.clientAdapter, Settings.CommonAddress);
            }
        }

        /// <summary>
        /// Ham doc single tag
        /// </summary>
        public bool Read(IOAddress ioAddress, out string value)
        {
            value = "0";

            // Tim trong buffer truoc (neu co Multi read)
            foreach (var blockReader in this.blockReaders)
            {
                if (blockReader.Contains(ioAddress.InformationObjectAddress))
                {
                    value = blockReader.GetValue(ioAddress.InformationObjectAddress);
                    return true;
                }
            }

            // Neu khong co trong buffer, doc truc tiep tu Device 
            if (!ConnectionStatus) return false;

            return this.clientAdapter.Read(Settings.CommonAddress, ioAddress, out object objValue) &&
                   ConvertValue(objValue, ioAddress.DataType, out value);
        }

        /// <summary>
        /// Ham ghi gia tri
        /// </summary>
        public bool Write(IOAddress ioAddress, string value)
        {
            if (!ConnectionStatus) return false;
            return this.clientAdapter.Write(Settings.CommonAddress, ioAddress, value);
        }

        private bool ConvertValue(object objValue, DataType dataType, out string value)
        {
            value = "0";
            try
            {
                if (objValue == null) return false;

                switch (dataType)
                {
                    case DataType.Bool:
                        value = Convert.ToBoolean(objValue) ? "1" : "0";
                        break;
                    case DataType.Word:
                    case DataType.Int:
                        value = Convert.ToInt32(objValue).ToString();
                        break;
                    case DataType.DWord:
                        value = Convert.ToUInt32(objValue).ToString();
                        break;
                    case DataType.Float:
                        value = Convert.ToSingle(objValue).ToString();
                        break;
                    default:
                        value = objValue.ToString();
                        break;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}