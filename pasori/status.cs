using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pasori
{
    public  class Status
    {
        //運転前か後か　後ならフラグ
        private static Boolean driveStatus;
        //アルコール反応ありか、ありならフラグ
        private static Boolean alcoholFlag;
        //チェックする人の電子錠ID
        private static string checkerId;
        //立会人の電子錠ID
        string witnessId;
        //チェック方法
        string checkMethod;
        //顔色　悪い場合フラグを立てる
        Boolean complexion;
        //体調　悪い場合フラグを立てる
        Boolean physicalCondition;
        //立会人の患者コード
        int code;

        public Boolean propertyDriveStatus
        {
            set { driveStatus = value; }
            get { return driveStatus; }
        }

        public Boolean propertyAlcoholFlag { 
            set { alcoholFlag = value; }
            get { return alcoholFlag; }
        }

        public string propertyCheckerId
        {
            set { checkerId = value; }
            get { return checkerId; }
        }

        public string propertyWitnessId
        {
            set { witnessId = value; }
            get { return witnessId; }
        }

        public string propertyCheckMethod
        {
            set { checkMethod = value; }
            get { return checkMethod; }
        }

        public Boolean propertyComplexion
        {
            set { complexion = value; }
            get { return complexion; }
        }

        public Boolean propertyPhysicalCondition
        {
            set { physicalCondition = value; }
            get { return physicalCondition; }
        }

        public int propertyCode
        {
            set { code = value; }
            get { return code; }
        }

    }


  
}
