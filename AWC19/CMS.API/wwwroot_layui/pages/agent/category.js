function del(nodeId) {
	alert(nodeId)
}
/*分类-停用*/
function member_stop(obj, id) {
	var confirmTip;
	if($(obj).attr('title') == '启用') {
		confirmTip = '确认要停用吗？';
	} else {
		confirmTip = '确认要启用吗？';
	}
	layer.confirm(confirmTip, function(index) {
		if($(obj).attr('title') == '启用') {
			//发异步把用户状态进行更改
			$(obj).attr('title', '停用')
			$(obj).find('i').html('&#xe62f;');
			$(obj).parents("tr").find(".td-status").find('span').addClass('layui-btn-disabled').html('已停用');
			layer.msg('已停用!', {
				icon: 5,
				time: 1000
			});
		} else {
			$(obj).attr('title', '启用')
			$(obj).find('i').html('&#xe601;');

			$(obj).parents("tr").find(".td-status").find('span').removeClass('layui-btn-disabled').html('已启用');
			layer.msg('已启用!', {
				icon: 6,
				time: 1000
			});
		}
	});
}
//自定义的render渲染输出多列表格
var layout = [{
		name: '菜单名称',
		treeNodes: true,
		headerClass: 'value_col',
		colClass: 'value_col',
	},
	{
		name: '姓名',
		field:'username'
	},
	{
		name: '生日',
		field: 'birthday'
	},
	{
		name: '注册网址',
		field: 'website'
	},
	{
		name: '注册IP',
		field: 'ip'
	},
	{
		name: '注册时间',
		field: 'time'
	},
	{
		name: '级别',
		field: 'level'
	},
	{
		name: '状态',
		headerClass: 'td-status',
		colClass: 'td-status',
		style: 'width: 10%',
		render: function(row) {
			return '<span class="layui-btn layui-btn-normal layui-btn-xs">已启用</span>';
		}
	},
	{
		name: '管理',
		headerClass: 'td-manage',
		colClass: 'td-manage',
		render: function(row) {
			return '<a onclick="member_stop(this,\'10001\')" href="javascript:;" title="启用"><i class="layui-icon">&#xe601;</i></a>' +
				'<a title="编辑" onclick="WeAdminShow(\'编辑\',\'./agent_edit.html\')" href="javascript:;"><i class="layui-icon">&#xe642;</i></a>';
			//return '<a class="layui-btn layui-btn-danger layui-btn-mini" onclick="del(' + row.id + ')"><i class="layui-icon">&#xe640;</i> 删除</a>'; //列渲染
		}
	},
];
//加载扩展模块 treeGird
//		layui.config({
//			  base: './static/js/'
//			  ,version: '101100'
//			}).use('admin');
layui.extend({
	admin: '{/}../../static/js/admin',
	treeGird: '{/}../../lib/layui/lay/treeGird' // {/}的意思即代表采用自有路径，即不跟随 base 路径
});
layui.use(['treeGird', 'jquery', 'admin', 'layer'], function() {
	var layer = layui.layer,
		$ = layui.jquery,
		admin = layui.admin,
		treeGird = layui.treeGird;

	var tree1 = layui.treeGird({
		elem: '#demo', //传入元素选择器
		spreadable: false, //设置是否全展开，默认不展开
		nodes: [{
				"id": "1",
				"name": "1级代理",
				"username": "钱百万",
				"birthday": "1995-10-10",
				"website": "www.baidu.com",
				"ip": "192.1.1.1",
				"time": "2019-01-10",
				"level": "万象会员",
				"children": [{
						"id": "11",
						"name": "二级代理",
						"username": "马牛逼",
						"birthday": "1995-10-10",
						"website": "www.baidu.com",
						"ip": "192.1.1.1",
						"time": "2019-01-10",
						"level": "万象会员",
						"children": [{
								"id": "12",
								"name": "三级代理",
								"username": "马牛逼",
								"birthday": "1995-10-10",
								"website": "www.baidu.com",
								"ip": "192.1.1.1",
								"time": "2019-01-10",
								"level": "万象会员",
								"children": [{
										"id": "13",
										"name": "4级代理",
										"username": "马牛逼",
										"birthday": "1995-10-10",
										"website": "www.baidu.com",
										"ip": "192.1.1.1",
										"time": "2019-01-10",
										"level": "万象会员",
										"children": [{
												"id": "14",
												"name": "5级代理",
												"username": "马牛逼",
												"birthday": "1995-10-10",
												"website": "www.baidu.com",
												"ip": "192.1.1.1",
												"time": "2019-01-10",
												"level": "万象会员",
											},
										]
									},
								]
							},
						]
					},{
						"id": "1111",
						"name": "二级代理",
						"username": "马牛逼",
						"birthday": "1995-10-10",
						"website": "www.baidu.com",
						"ip": "192.1.1.1",
						"time": "2019-01-10",
						"level": "万象会员",
						"children": [{
								"id": "22",
								"name": "三级代理",
								"username": "马牛逼",
								"birthday": "1995-10-10",
								"website": "www.baidu.com",
								"ip": "192.1.1.1",
								"time": "2019-01-10",
								"level": "万象会员",
								"children": [{
										"id": "23",
										"name": "4级代理",
										"username": "马牛逼",
										"birthday": "1995-10-10",
										"website": "www.baidu.com",
										"ip": "192.1.1.1",
										"time": "2019-01-10",
										"level": "万象会员",
										"children": [{
												"id": "24",
												"name": "5级代理",
												"username": "马牛逼",
												"birthday": "1995-10-10",
												"website": "www.baidu.com",
												"ip": "192.1.1.1",
												"time": "2019-01-10",
												"level": "万象会员",
											},
										]
									},
								]
							},
						]
					},
				]
			},
			{
				"id": "2",
				"name": "父节点2",
				"children": [{
					"id": "21",
					"name": "子节点21",
					"children": [{
						"id": "211",
						"name": "子节点211"
					}]
				}]
			}
		],
		layout: layout
	});
	$('#collapse').on('click', function() {
		layui.collapse(tree1);
	});

	$('#expand').on('click', function() {
		layui.expand(tree1);
	});
});