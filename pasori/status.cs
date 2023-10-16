using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pasori
{
    public  class Status
    {
        
        //本人の職員ｺｰﾄﾞ
        private string code;
        //本人の名前
        private string name;

        //車のナンバー
        private string carNumber;
        //車種
        private string carName;

        //運転前か後か　後ならフラグ
        private Boolean driveStatus;
        //アルコール反応ありか、ありならフラグ
        private Boolean alcoholFlag;
        //チェックする人の電子錠ID
        private static string checkerId;
        //チェック方法
        private Boolean checkMethod;
        //顔色　悪い場合フラグを立てる
        private Boolean complexion;
        //体調　悪い場合フラグを立てる
        private Boolean physicalCondition;
        //コメント　
        private string comment;

        //立会人の職員コード
        private string witnesscode;
        //立会人の名前
        private string witnessName;

        

        public string propertyCarNumber
        {
            set { carNumber = value; }
            get { return carNumber; }
        }

        public string propertyCarName
        {
            set { carName = value; }
            get { return carName; }
        }

        public string propertyCode
        {
            set { code = value; }
            get { return code; }
        }
        public string propertyName
        {
            set { name = value; }
            get { return name; }
        }

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

        public Boolean propertyCheckMethod
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

        public string propertyWitnessCode
        {
            set { witnesscode = value; }
            get { return witnesscode; }
        }
        
        public string propertyWitnessName
        {
            set { witnessName = value; }
            get { return witnessName; }
        }

        public string propertyComment
        {
            set { comment = value; }
            get { return comment; }
        }
    }


  
}
