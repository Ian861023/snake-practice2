from flask import Flask, request, jsonify,Response,render_template
import json
from flask_sqlalchemy import SQLAlchemy
from flask_jwt_extended import create_access_token, JWTManager,jwt_required
from werkzeug.security import check_password_hash, generate_password_hash
from flask_cors import CORS
app = Flask(__name__, static_folder='static')

# 連接 MySQL 資料庫
app.config['SQLALCHEMY_DATABASE_URI'] = 'mysql+mysqlconnector://root:%409861023@localhost/sql_tutorial'
app.config['SQLALCHEMY_TRACK_MODIFICATIONS'] = False
app.config['JWT_SECRET_KEY'] = 'your_jwt_secret_key'
CORS(app)
#初始化
db=SQLAlchemy(app)
jwt = JWTManager(app)
class User(db.Model):
    __tablename__ = 'users'

    id = db.Column(db.Integer, primary_key=True)
    username = db.Column(db.String(100), unique=True, nullable=False)
    password = db.Column(db.String(100), nullable=False)

    def __repr__(self):
        return f"<User {self.username}>"
class GameScore(db.Model):
    __tablename__ = 'snakescore'  # 資料表名稱

    snake_id = db.Column(db.Integer, primary_key=True)
    snakename = db.Column(db.String(100), nullable=False)
    score = db.Column(db.Integer, nullable=False)

    def __repr__(self):
        return f"<GameScore {self.snakename} - {self.score}>"


@app.route('/register', methods=['POST'])
def register():
    data = request.get_json()
    username = data.get('username')
    password = data.get('password')

    if not username or not password:
        return jsonify({"error": "Username and password are required"}), 400

    # 檢查用戶是否已經存在
    user = User.query.filter_by(username=username).first()
    if user:
        return jsonify({"error": "User already exists"}), 400
    hashed_password = generate_password_hash(password)#加密

    new_user = User(username=username, password=hashed_password)  # 建立新用戶
    db.session.add(new_user)
    db.session.commit()

    return jsonify({"message": "User created successfully!"}), 201
@app.route('/login.html', methods=['GET'])
def login_page():
    # 渲染登入頁面
    return render_template('login.html')
@app.route("/login", methods=["POST"])
def login():
    
    data = request.get_json()
    username = data.get("username")
    password = data.get("password")

    user = User.query.filter_by(username=username).first()

    if user and check_password_hash(user.password, password):#哈希密碼比對
        token = create_access_token(identity=username)
        return jsonify({"token": token}), 200

    return jsonify({"error": "Invalid credentials"}), 401
@app.route('/')
def index():
    return render_template('register.html')
@app.route('/index.html', methods=['GET'])
def index_page():
    # 渲染登入頁面
    return render_template('index.html')
#解析json送到sql
@app.route('/api/games', methods=['POST'])
def add_game_score():
    # 解析 JSON
    data = request.get_json()
    name = data.get('snakename')
    score = data.get('score')
    if not name or not score:
        return jsonify({"error": "Name and score are required"}), 400

    new_score = GameScore(snakename=name, score=score)  # 創建 GameScore 實例

    try:
        db.session.add(new_score)  # 加入到會話
        db.session.commit()  # 提交到資料庫
        return jsonify({"message": "Score added successfully!"}), 201
    except Exception as e:
        db.session.rollback()  # 發生錯誤時回滾
        return jsonify({"error": f"Failed to add score: {str(e)}"}), 500

   
#傳到html
@app.route('/api/games', methods=['GET'])
def show_scores():
    try:
        scores = GameScore.query.all()  # 查詢所有分數
        formatted_scores = [{"snakename": score.snakename, "score": score.score} for score in scores]
        return jsonify(formatted_scores)
    except Exception as e:
        return jsonify({"error": f"An error occurred: {str(e)}"}), 500

#更新
@app.route('/api/games', methods=['PUT'])
@jwt_required()
def update_game_score():
    # 解析 JSON 資料
    data = request.get_json()

    # 確保收到的資料包含 snakename 和 score
    snakename = data.get('snakename')
    score = data.get('score')

    if not snakename or not score:
        return jsonify({"error": "Both snakename and score are required"}), 400

    # 在資料庫中查找對應的玩家並更新分數
    game_score = GameScore.query.filter_by(snakename=snakename).first()  # 查找玩家

    if game_score:
        game_score.score = score  # 更新分數
        db.session.commit()  # 提交更改
        return jsonify({"message": "Score updated successfully!"}), 200
    else:
        return jsonify({"error": "Player not found"}), 404
@app.route('/api/games', methods=['DELETE'])
@jwt_required()
def delete_score():
    data = request.get_json()
    snakename = data.get('snakename')  # 玩家名称
    score = data.get('score')  # 分数

    if not snakename or not score:
        return jsonify({"error": "Player name and score are required"}), 400

    game_score = GameScore.query.filter_by(snakename=snakename, score=score).first()  # 查找玩家

    if game_score:
        db.session.delete(game_score)  # 刪除分數
        db.session.commit()  # 提交更改
        return jsonify({"message": "Score deleted successfully."}), 200
    else:
        return jsonify({"error": "No matching player and score found."}), 404
# 啟動 Flask 伺服器
if __name__ == '__main__':
    with app.app_context():
        db.create_all()

    app.run(debug=True)
