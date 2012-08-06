namespace NSoft.NFramework.DataServices.WinForm
{
	partial class Form1
	{
		/// <summary>
		/// 필수 디자이너 변수입니다.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 사용 중인 모든 리소스를 정리합니다.
		/// </summary>
		/// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form 디자이너에서 생성한 코드

		/// <summary>
		/// 디자이너 지원에 필요한 메서드입니다.
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
		/// </summary>
		private void InitializeComponent()
		{
			this.panel1 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.txtMethod = new System.Windows.Forms.TextBox();
			this.btnExecute = new System.Windows.Forms.Button();
			this.txtResultCount = new System.Windows.Forms.TextBox();
			this.lblResultCount = new System.Windows.Forms.Label();
			this.listView = new System.Windows.Forms.ListView();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.btnExecute);
			this.panel1.Controls.Add(this.txtMethod);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Location = new System.Drawing.Point(12, 15);
			this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(658, 48);
			this.panel1.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(14, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(43, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "메소드";
			// 
			// txtMethod
			// 
			this.txtMethod.Location = new System.Drawing.Point(63, 14);
			this.txtMethod.Name = "txtMethod";
			this.txtMethod.Size = new System.Drawing.Size(451, 23);
			this.txtMethod.TabIndex = 1;
			this.txtMethod.Text = "Customer, GetAll";
			// 
			// btnExecute
			// 
			this.btnExecute.Location = new System.Drawing.Point(533, 13);
			this.btnExecute.Name = "btnExecute";
			this.btnExecute.Size = new System.Drawing.Size(122, 23);
			this.btnExecute.TabIndex = 2;
			this.btnExecute.Text = "실 행";
			this.btnExecute.UseVisualStyleBackColor = true;
			this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
			// 
			// txtResultCount
			// 
			this.txtResultCount.Location = new System.Drawing.Point(92, 581);
			this.txtResultCount.Name = "txtResultCount";
			this.txtResultCount.Size = new System.Drawing.Size(221, 23);
			this.txtResultCount.TabIndex = 3;
			// 
			// lblResultCount
			// 
			this.lblResultCount.AutoSize = true;
			this.lblResultCount.Location = new System.Drawing.Point(11, 584);
			this.lblResultCount.Name = "lblResultCount";
			this.lblResultCount.Size = new System.Drawing.Size(75, 15);
			this.lblResultCount.TabIndex = 2;
			this.lblResultCount.Text = "ResultCount:";
			// 
			// listView
			// 
			this.listView.Location = new System.Drawing.Point(19, 91);
			this.listView.Name = "listView";
			this.listView.Size = new System.Drawing.Size(636, 465);
			this.listView.TabIndex = 4;
			this.listView.UseCompatibleStateImageBehavior = false;
			this.listView.View = System.Windows.Forms.View.Details;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(682, 616);
			this.Controls.Add(this.listView);
			this.Controls.Add(this.txtResultCount);
			this.Controls.Add(this.lblResultCount);
			this.Controls.Add(this.panel1);
			this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "Form1";
			this.Text = "Form1";
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button btnExecute;
		private System.Windows.Forms.TextBox txtMethod;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtResultCount;
		private System.Windows.Forms.Label lblResultCount;
		private System.Windows.Forms.ListView listView;
	}
}

